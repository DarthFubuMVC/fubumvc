using System;
using System.Collections.Generic;
using System.Web.Http.SelfHost;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.SelfHost
{
    public class SelfHostHttpServer : IDisposable
    {
        private HttpSelfHostConfiguration _configuration;
        private HttpSelfHostServer _server;
        private string _baseAddress;
        private int _port;

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
                if (e.InnerException is System.ServiceModel.AddressAlreadyInUseException)
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

            _server = new HttpSelfHostServer(_configuration, new SelfHostHttpMessageHandler(runtime, _configuration){
                Verbose = Verbose
            });

            runtime.Facility.Register(typeof(HttpSelfHostConfiguration), ObjectDef.ForValue(_configuration));

            Console.WriteLine("Starting to listen for requests at " + _configuration.BaseAddress);

            _server.OpenAsync().Wait();
        }

        public void Dispose()
        {
            _server.CloseAsync().ContinueWith(t => _server.SafeDispose()).Wait();
        }
    }
}