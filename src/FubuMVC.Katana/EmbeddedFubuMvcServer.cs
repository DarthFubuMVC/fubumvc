using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Urls;
using FubuMVC.OwinHost;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Hosting.Builder;
using Microsoft.Owin.Hosting.Engine;
using Microsoft.Owin.Hosting.Loader;
using Microsoft.Owin.Hosting.ServerFactory;
using Microsoft.Owin.Hosting.Services;
using Microsoft.Owin.Hosting.Tracing;
using Owin;

namespace FubuMVC.Katana
{
    /// <summary>
    /// Embeds and runs a FubuMVC application in a normal process using the Web API self host libraries
    /// </summary>
    public class EmbeddedFubuMvcServer : IDisposable
    {
        private IDisposable _server;
        private IUrlRegistry _urls;
        private IServiceLocator _services;
        private EndpointDriver _endpoints;
        private string _baseAddress;
        private readonly FubuRuntime _runtime;

        /// <summary>
        /// Creates an embedded FubuMVC server for the designated application source
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="physicalPath">The physical path of the web server path.  This only needs to be set if the location for application content like scripts or views is at a different place than the current AppDomain base directory.  If this value is blank, the embedded server will attempt to find a folder with the same name as the assembly that contains the IApplicationSource</param>
        /// <param name="port">The port to run the web server at.  The web server will try other port numbers starting at this point if it is unable to bind to this specific port</param>
        /// <returns></returns>
        public static EmbeddedFubuMvcServer For<T>(string physicalPath = null, int port = 5500) where T : IApplicationSource, new()
        {
            if (physicalPath.IsEmpty())
            {
                physicalPath = TryToGuessApplicationPath(typeof (T)) ?? AppDomain.CurrentDomain.BaseDirectory;
            }

            return new EmbeddedFubuMvcServer(new T().BuildApplication().Bootstrap(), physicalPath, port);

        }

        public static string TryToGuessApplicationPath(Type type)
        {
            var solutionFolder = AppDomain.CurrentDomain.BaseDirectory.ParentDirectory().ParentDirectory().ParentDirectory();
            var applicationFolder = solutionFolder.AppendPath(type.Assembly.GetName().Name);

            if (Directory.Exists(applicationFolder)) return applicationFolder;

            return null;
        }


        public EmbeddedFubuMvcServer(FubuRuntime runtime, string physicalPath = null, int port = 5500)
        {
            _runtime = runtime;

            // before anything else, make sure there is no server on the settings
            // We're doing this hokey-pokey to ensure that things don't get double 
            // disposed
            var settings = runtime.Factory.Get<KatanaSettings>();
            var peer = settings.EmbeddedServer;

            if (peer == null)
            {
                startAllNew(runtime, physicalPath, port);
            }
            else
            {
                takeOverFromExistingServer(peer, settings);
            }
        }

        private void startAllNew(FubuRuntime runtime, string physicalPath, int port)
        {
            startServer(runtime.Routes, physicalPath, port);

            _urls = _runtime.Factory.Get<IUrlRegistry>();
            _services = _runtime.Factory.Get<IServiceLocator>();

            buildEndpointDriver(port);
        }

        private void takeOverFromExistingServer(EmbeddedFubuMvcServer peer, KatanaSettings settings)
        {
            _urls = peer.Urls;
            _services = peer.Services;
            _server = peer._server;
            _baseAddress = peer._baseAddress;
            _endpoints = peer.Endpoints;

            settings.EmbeddedServer = null;
        }

        private void buildEndpointDriver(int port)
        {
            _baseAddress = "http://localhost:" + port;
            UrlContext.Stub(_baseAddress);
            _endpoints = new EndpointDriver(_urls, _baseAddress);
        }

        public EmbeddedFubuMvcServer(KatanaSettings settings, IUrlRegistry urls, IServiceLocator services, IList<RouteBase> routes)
        {
            _urls = urls;
            _services = services;

            startServer(routes, AppDomain.CurrentDomain.BaseDirectory, settings.Port);
            buildEndpointDriver(settings.Port);
        }

        private void startServer(IList<RouteBase> routes, string physicalPath, int port)
        {
            var parameters = new StartOptions {Port = port};
            parameters.Urls.Add("http://*:" + port); //for netsh http add urlacl

            FubuMvcPackageFacility.PhysicalRootPath = physicalPath ?? AppDomain.CurrentDomain.BaseDirectory;
            Action<IAppBuilder> startup = FubuOwinHost.ToStartup(routes);

            var context = new StartContext(parameters)
            {
                Startup = startup
            };

            var engine = new HostingEngine(new AppBuilderFactory(), new TraceOutputFactory(),
                new AppLoader(new IAppLoaderFactory[0]),
                new ServerFactoryLoader(new ServerFactoryActivator(new ServiceProvider())));

            _server = engine.Start(context);
        }

        public FubuRuntime Runtime
        {
            get { return _runtime; }
        }

        public EndpointDriver Endpoints
        {
            get { return _endpoints; }
        }

        public IUrlRegistry Urls
        {
            get { return _urls; }
        }

        public IServiceLocator Services
        {
            get { return _services; }
        }

        public string PhysicalPath
        {
            get { return FubuMvcPackageFacility.GetApplicationPath(); }
        }

        public void Dispose()
        {
            if (_runtime != null) _runtime.Dispose();
            _server.Dispose();
        }

        public string BaseAddress
        {
            get { return _baseAddress; }
        }
    }
}