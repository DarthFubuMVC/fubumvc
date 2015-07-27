using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using Microsoft.Owin.Builder;
using Nowin;
using Owin;

namespace FubuMVC.Nowin
{
    /// <summary>
    /// Embeds and runs a FubuMVC application in a normal process using the Web API self host libraries
    /// </summary>
    public class EmbeddedFubuMvcServer : IDisposable
    {
        private IDisposable _server;
        private IUrlRegistry _urls;
        private IServiceFactory _services;
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
        public static EmbeddedFubuMvcServer For<T>(string physicalPath = null, int port = 5500)
            where T : IApplicationSource, new()
        {
            if (physicalPath.IsEmpty())
            {
                physicalPath = TryToGuessApplicationPath(typeof (T)) ?? AppDomain.CurrentDomain.BaseDirectory;
            }

            return new EmbeddedFubuMvcServer(new T().BuildApplication().Bootstrap(), physicalPath, port);
        }

        public static string TryToGuessApplicationPath(Type type)
        {
            var solutionFolder =
                AppDomain.CurrentDomain.BaseDirectory.ParentDirectory().ParentDirectory().ParentDirectory();
            var applicationFolder = solutionFolder.AppendPath(type.Assembly.GetName().Name);

            if (Directory.Exists(applicationFolder)) return applicationFolder;

            return null;
        }


        public EmbeddedFubuMvcServer(FubuRuntime runtime, string physicalPath = null, int port = 5500)
        {
            if (port <= 0)
            {
                port = PortFinder.FindPort(5500);
            }

            _runtime = runtime;
            _services = _runtime.Factory;

            // before anything else, make sure there is no server on the settings
            // We're doing this hokey-pokey to ensure that things don't get double
            // disposed
            var settings = runtime.Factory.Get<NowinSettings>();
            var peer = settings.EmbeddedServer;

            if (peer == null)
            {
                startAllNew(runtime, physicalPath, port);
            }
            else
            {
                takeOverFromExistingServer(peer, settings);
                port = settings.Port;
            }

            buildEndpointDriver(port);
        }


        private void startAllNew(FubuRuntime runtime, string physicalPath, int port)
        {
            startServer(runtime.Factory.Get<OwinSettings>(), physicalPath, port);

            _urls = _runtime.Factory.Get<IUrlRegistry>();
            _services = _runtime.Factory.Get<IServiceFactory>();

            buildEndpointDriver(port);
        }

        private void takeOverFromExistingServer(EmbeddedFubuMvcServer peer, NowinSettings settings)
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

        private void startServer(OwinSettings settings, string physicalPath, int port)
        {
            if (physicalPath != null) FubuApplication.PhysicalRootPath = physicalPath;

            var owinBuilder = new AppBuilder();
            OwinServerFactory.Initialize(owinBuilder.Properties);

            settings.EnvironmentData[OwinConstants.AppMode] = FubuMode.Mode().ToLower();
            settings.EnvironmentData.ToDictionary().Each(pair => owinBuilder.Properties.Add(pair));

            var host = new FubuOwinHost(_runtime.Routes);
            owinBuilder.Run(context => host.Invoke(context.Environment));

            var builder = ServerBuilder.New().SetPort(port).SetOwinApp(owinBuilder.Build());
            _server = builder.Start();
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

        public IServiceFactory Services
        {
            get { return _services; }
        }

        public string PhysicalPath
        {
            get { return _runtime.Files.RootPath; }
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