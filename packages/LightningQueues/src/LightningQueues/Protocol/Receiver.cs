using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using LightningQueues.Exceptions;
using LightningQueues.Logging;
using LightningQueues.Model;

namespace LightningQueues.Protocol
{
    public class Receiver : IDisposable
    {
        private readonly ILogger _logger;
        private readonly IPEndPoint _endpointToListenTo;
        private readonly Func<Message[], IMessageAcceptance> _acceptMessages;
        private TcpListener _listener;
        private bool _disposed;

        public Receiver(IPEndPoint endpointToListenTo, Func<Message[], IMessageAcceptance> acceptMessages, ILogger logger = null)
        {
            _endpointToListenTo = endpointToListenTo;
            _acceptMessages = acceptMessages;
            _logger = logger ?? LogManager.GetLogger<Receiver>();
            Timeout = TimeSpan.FromSeconds(5);
        }

        public TimeSpan Timeout { get; set; }

        public void Start()
        {
            _logger.Debug("Starting to listen on {0}", _endpointToListenTo);
            TryStart(_endpointToListenTo);
            StartAccepting();
            _logger.Debug("Now listen on {0}", _endpointToListenTo);
        }

        private void StartAccepting()
        {
            Task.Factory.StartNew(async () =>
            {
                while (!_disposed)
                {
                    await AcceptTcpClientAsync().ConfigureAwait(false);
                }
            }, TaskCreationOptions.LongRunning);
        }

        private void TryStart(IPEndPoint endpointToListenTo)
        {
            try
            {
                _listener = new TcpListener(endpointToListenTo);
                _listener.Start();
            }
            catch (SocketException ex)
            {
                if (ex.Message == "Only one usage of each socket address (protocol/network address/port) is normally permitted")
                {
                    throw new EndpointInUseException(endpointToListenTo, ex);
                }
            }
        }

        private async Task AcceptTcpClientAsync()
        {
            try
            {
                var client = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);
                _logger.Debug("Accepting connection from {0}", client.Client.RemoteEndPoint);
                ProcessRequest(client);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception ex)
            {
                _logger.Info("Error on AcceptTcpClientAsync" + ex.Message, ex);
            }
        }

        private async Task ProcessRequest(TcpClient client)
        {
            var sender = client.Client.RemoteEndPoint.ToString();
            try
            {
                using (client)
                using (var stream = client.GetStream())
                {
                    await new ReceivingProtocol().ReadMessagesAsync(sender, stream, _acceptMessages)
                        .WithTimeout(Timeout)
                        .ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Error on ProcessRequest for endpoint {0}", ex, sender);
            }
        }

        public void Dispose()
        {
            _disposed = true;
            _listener.Stop();
        }
    }
}