using System;
using System.ServiceModel;
using System.Web.Http.SelfHost;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.SelfHost
{
    public class SelfHostHttpServer : IDisposable
    {
        private string _baseAddress;
        private HttpSelfHostConfiguration _configuration;
        private int _port;
        private readonly string _rootDirectory;
        private SelfHostHttpMessageHandler _selfHostHttpMessageHandler;
        private HttpSelfHostServer _server;

        public SelfHostHttpServer(int port, string rootDirectory)
        {
            _port = port;
            _rootDirectory = rootDirectory;
        }

        public string BaseAddress
        {
            get { return _baseAddress; }
        }

        public int Port
        {
            get { return _port; }
        }

        public bool Verbose { get; set; }

        public HttpSelfHostConfiguration Configuration
        {
            get { return _configuration; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            close();
        }

        #endregion

        public void Start(FubuRuntime runtime)
        {
            _port = PortFinder.FindPort(_port);

            int i = 0;
            while (!tryStartAtPort(runtime) && i > 3)
            {
                i++;
            }
        }

        public void Recycle(FubuRuntime runtime)
        {
            close();
            Start(runtime);
        }

        private bool tryStartAtPort(FubuRuntime runtime)
        {
            try
            {
                startAtPort(runtime);
                return true;
            }
            catch (AggregateException e)
            {
                if (e.InnerException is AddressAlreadyInUseException)
                {
                    _port++;
                    return false;
                }

                throw;
            }
        }

        private void startAtPort(FubuRuntime runtime)
        {
            _baseAddress = "http://localhost:" + _port;
            _configuration = new HttpSelfHostConfiguration(_baseAddress);

            FubuMvcPackageFacility.PhysicalRootPath = _rootDirectory;

            _selfHostHttpMessageHandler = new SelfHostHttpMessageHandler(runtime)
            {
                Verbose = Verbose
            };

            _server = new HttpSelfHostServer(_configuration, _selfHostHttpMessageHandler);

            runtime.Facility.Register(typeof (HttpSelfHostConfiguration), ObjectDef.ForValue(_configuration));

            Console.WriteLine("Starting to listen for requests at " + _configuration.BaseAddress);

            _server.OpenAsync().Wait();
        }

        private void close()
        {
            _server.CloseAsync().ContinueWith(t => _server.SafeDispose()).Wait();
        }
    }
}