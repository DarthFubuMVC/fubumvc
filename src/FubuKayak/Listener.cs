using System;
using System.Net;
using System.Threading;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.OwinHost;
using Gate;
using Gate.Kayak;
using Kayak;

namespace FubuKayak
{
    public class Listener : IDisposable
    {
        private readonly FubuOwinHost _host = new FubuOwinHost();
        private readonly IPEndPoint _listeningEndpoint;
        private readonly AppDelegate _applicationDelegate;
        private readonly ISchedulerDelegate _schedulerDelegate;
        private readonly IScheduler _scheduler;
        private readonly IServer _server;
        private IDisposable _kayakListenerDisposer;

        public Listener(int port) : this(new IPEndPoint(IPAddress.Any, port), new SchedulerDelegate())
        {
            
        }

        public Listener(IPEndPoint listeningEndpoint, ISchedulerDelegate schedulerDelegate)
        {
            _listeningEndpoint = listeningEndpoint;
            _schedulerDelegate = schedulerDelegate;
            _applicationDelegate = AppBuilder.BuildConfiguration(x => RunExtensions.Run(x.RescheduleCallbacks(), _host.ExecuteRequest));

            _scheduler = KayakScheduler.Factory.Create(_schedulerDelegate);
            _server = KayakServer.Factory.CreateGate(_applicationDelegate, _scheduler, null);
        }

        public bool Verbose
        {
            get { return _host.Verbose; }
            set { _host.Verbose = value; }
        }

        public void Start(FubuRuntime runtime, Action action)
        {
            Console.WriteLine("Starting to listen for requests at http://localhost:" + _listeningEndpoint.Port);
            _kayakListenerDisposer = _server.Listen(_listeningEndpoint);
            _scheduler.Post(() => ThreadPool.QueueUserWorkItem(o => action()));
            _scheduler.Start();
        }

        public ManualResetEvent StartOnNewThread(FubuRuntime runtime, Action action)
        {
            var reset = new ManualResetEvent(false);

            ThreadPool.QueueUserWorkItem(o =>
            {
                Start(runtime, () =>
                {
                    action();
                    reset.Set();
                });
            });

            return reset;
        }

        public void Stop()
        {
            try
            {
                Console.WriteLine("Stopping the Kayak scheduler");

                _scheduler.Stop();
            }
            catch (Exception)
            {
                // That's right, shut this puppy down
            }

            _server.SafeDispose();
            _kayakListenerDisposer.SafeDispose();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}