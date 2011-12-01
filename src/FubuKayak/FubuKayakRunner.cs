using System;
using System.Net;
using System.Threading;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.OwinHost;
using Gate;
using Gate.Kayak;
using Kayak;

namespace FubuKayak
{
    public class FubuKayakRunner
    {
        private readonly IApplicationSource _source;
        private readonly ISchedulerDelegate _schedulerDelegate;
        private IPEndPoint _listeningEndpoint;
        private AppDelegate _applicationDelegate;
        private IScheduler _scheduler;
        private IServer _server;
        private int _port;
        private readonly FubuOwinHost _host = new FubuOwinHost();
        private bool _latched;
        private IDisposable _kayakListenerDisposer;

        public FubuKayakRunner(IApplicationSource source) : this(source, new SchedulerDelegate())
        {
        }

        public FubuKayakRunner(IApplicationSource source, ISchedulerDelegate schedulerDelegate)
        {
            _source = source;
            _schedulerDelegate = schedulerDelegate;
        }

        public bool Verbose
        {
            get { return _host.Verbose; }
            set { _host.Verbose = value; }
        }

        public void RunApplication(int port, Action<FubuRuntime> activation)
        {
            _port = port;


            _listeningEndpoint = new IPEndPoint(IPAddress.Any, _port);
            Console.WriteLine("Listening on " + _listeningEndpoint);

            _applicationDelegate = AppBuilder.BuildConfiguration(x => x.RescheduleCallbacks().Run(_host.ExecuteRequest));

            _scheduler = KayakScheduler.Factory.Create(_schedulerDelegate);
            _server = KayakServer.Factory.CreateGate(_applicationDelegate, _scheduler, null);

            if (_listeningEndpoint == null)
            {
                throw new InvalidOperationException("Start() can only be called after RunApplication() and Stop()");
            }

            var runtime = rebuildFubuMVCApplication();

            _kayakListenerDisposer = _server.Listen(_listeningEndpoint);
                _scheduler.Post(() => ThreadPool.QueueUserWorkItem(o => activation(runtime)));
                _scheduler.Start();


            
        }

        private FubuRuntime rebuildFubuMVCApplication()
        {
            RouteTable.Routes.Clear();
            return _source.BuildApplication().Bootstrap();
        }

        public void Stop()
        {
            try
            {
                _scheduler.Stop();

            }
            catch (Exception)
            {
                // That's right, shut this puppy down
            }

            _server.SafeDispose();
        }

        public void Recycle(Action<FubuRuntime> activation)
        {
            _latched = true;
            var runtime = rebuildFubuMVCApplication();
            _latched = false;

            activation(runtime);
        }




    }

}