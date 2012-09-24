using System;
using System.Web.Http.SelfHost;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.SelfHost
{
    public class SelfHostHttpServer : IDisposable
    {
        private readonly HttpSelfHostConfiguration _configuration;
        private HttpSelfHostServer _server;
        private readonly string _baseAddress;

        public SelfHostHttpServer(int port)
        {
            port = PortFinder.FindPort(port);
            _baseAddress = "http://localhost:" + port;
            _configuration = new HttpSelfHostConfiguration(_baseAddress);
        }

        public string BaseAddress
        {
            get { return _baseAddress; }
        }

        

        public bool Verbose { get; set; }

        public HttpSelfHostConfiguration Configuration
        {
            get { return _configuration; }
        }

        public void Start(FubuRuntime runtime, string rootDirectory)
        {
            FubuMvcPackageFacility.PhysicalRootPath = rootDirectory;

            _server = new HttpSelfHostServer(_configuration, new SelfHostHttpMessageHandler(runtime, _configuration){
                Verbose = Verbose
            });

            runtime.Facility.Register(typeof(HttpSelfHostConfiguration), ObjectDef.ForValue(_configuration));

            Console.WriteLine("Starting to listen for requests at " + _configuration.BaseAddress);

            _server.OpenAsync().Wait();
        }

        public void Dispose()
        {
            _server.CloseAsync().ContinueWith(t => _server.SafeDispose());
        }
    }
}