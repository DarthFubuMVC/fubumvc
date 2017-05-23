using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using FubuCore.Logging;

namespace FubuMVC.LightningQueues.Queues.Net.Tcp
{
    public class Receiver : IDisposable
    {
        readonly TcpListener _listener;
        readonly IReceivingProtocol _protocol;
        private readonly ILogger _logger;
        bool _disposed;
        IObservable<Message> _stream;
        private readonly object _lockObject;
        
        public Receiver(IPEndPoint endpoint, IReceivingProtocol protocol, ILogger logger)
        {
            Endpoint = endpoint;
            Timeout = TimeSpan.FromSeconds(5);
            _protocol = protocol;
            _logger = logger;
            _listener = new TcpListener(Endpoint);
            _listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _lockObject = new object();
        }

        public TimeSpan Timeout { get; set; }

        public IPEndPoint Endpoint { get; }

        public IObservable<Message> StartReceiving()
        {
            lock (_lockObject)
            {
                if (_stream != null)
                    return _stream;

                _listener.Start();

                _logger.Debug($"TcpListener started listening on port: {Endpoint.Port}");
                _stream = Observable.While(IsNotDisposed, ContinueAcceptingNewClients())
                    .Using(x => _protocol.ReceiveStream(Observable.Return(new NetworkStream(x, true)), x.RemoteEndPoint.ToString())
                    .Catch((Exception ex) => catchAll(ex)))
                    .Catch((Exception ex) => catchAll(ex))
                    .Publish()
                    .RefCount()
                    .Finally(() => _logger.Info($"TcpListener at {Endpoint.Port} has stopped"))
                    .Catch((Exception ex) => catchAll(ex));
            }
            return _stream;
        }

        private IObservable<Message> catchAll(Exception ex)
        {
            _logger.Error("Error in message receiving", ex);
            return Observable.Empty<Message>();
        }

        private bool IsNotDisposed()
        {
            return !_disposed;
        }

        private IObservable<Socket> ContinueAcceptingNewClients()
        {
            return Observable.FromAsync(() => _listener.AcceptSocketAsync())
                .Do(x => _logger.Debug($"Client at {x.RemoteEndPoint} connection established."))
                .Repeat();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _logger.Info($"Disposing TcpListener at {Endpoint.Port}");
                _disposed = true;
                _listener.Stop();
            }
        }
    }
}