using System;
using System.ServiceModel;
using System.Threading.Tasks;
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
        private SelfHostHttpMessageHandler _selfHostHttpMessageHandler;
        private HttpSelfHostServer _server;

        public SelfHostHttpServer(int port)
        {
            _port = port;
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
            _server.CloseAsync().ContinueWith(t => _server.SafeDispose()).Wait();
        }

        #endregion

        public void Start(FubuRuntime runtime, string rootDirectory)
        {
            _port = PortFinder.FindPort(_port);

            int i = 0;
            while (!tryStartAtPort(rootDirectory, runtime) && i > 3)
            {
                i++;
            }
        }

        private bool tryStartAtPort(string rootDirectory, FubuRuntime runtime)
        {
            try
            {
                startAtPort(rootDirectory, runtime);
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

        private void startAtPort(string rootDirectory, FubuRuntime runtime)
        {
            _baseAddress = "http://localhost:" + _port;
            _configuration = new HttpSelfHostConfiguration(_baseAddress);

            FubuMvcPackageFacility.PhysicalRootPath = rootDirectory;

            _selfHostHttpMessageHandler = new SelfHostHttpMessageHandler(runtime)
            {
                Verbose = Verbose
            };

            _server = new HttpSelfHostServer(_configuration, _selfHostHttpMessageHandler);

            runtime.Facility.Register(typeof (HttpSelfHostConfiguration), ObjectDef.ForValue(_configuration));

            Console.WriteLine("Starting to listen for requests at " + _configuration.BaseAddress);

            _server.OpenAsync().Wait();
        }

        public Task Recycle(FubuRuntime runtime)
        {
            return _server.CloseAsync().ContinueWith(t => {
                _selfHostHttpMessageHandler.ReplaceRuntime(runtime);
            }).ContinueWith(t => _server.OpenAsync());

        }
    }
}