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
using Microsoft.Owin.Hosting.Settings;
using Owin;

namespace FubuMVC.Katana
{
    public static class FubuApplicationExtensions
    {
        /// <summary>
        /// Creates an embedded web server for this FubuApplication running at the designated physical path and port
        /// </summary>
        /// <param name="application"></param>
        /// <param name="physicalPath">The physical path of the web server path.  This only needs to be set if the location for application content like scripts or views is at a different place than the current AppDomain base directory</param>
        /// <param name="port">The port to run the web server at.  The web server will try other port numbers starting at this point if it is unable to bind to this specific port</param>
        /// <returns></returns>
        public static EmbeddedFubuMvcServer RunEmbedded(this FubuApplication application, string physicalPath = null,
                                                        int port = 5500)
        {
            return new EmbeddedFubuMvcServer(application.Bootstrap(), physicalPath, port);
        }
    }

    /// <summary>
    /// Embeds and runs a FubuMVC application in a normal process using the Web API self host libraries
    /// </summary>
    public class EmbeddedFubuMvcServer : IDisposable
    {
        private readonly IDisposable _server;
        private readonly IUrlRegistry _urls;
        private readonly IServiceLocator _services;
        private readonly EndpointDriver _endpoints;
        private readonly string _baseAddress;
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


        public class Starter
        {
            private readonly IList<RouteBase> _routes;

            public Starter(FubuRuntime runtime)
            {
                _routes = runtime.Routes;
            }

            public void Configuration(IAppBuilder builder)
            {
                var host = new FubuOwinHost(_routes);
                builder.Run(host);
            }


        }

        public EmbeddedFubuMvcServer(FubuRuntime runtime, string physicalPath = null, int port = 5500, StartParameters parameters = null)
        {
            _runtime = runtime;

            parameters = parameters ?? new StartParameters();
            parameters.Port = port;

            FubuMvcPackageFacility.PhysicalRootPath = physicalPath ?? AppDomain.CurrentDomain.BaseDirectory;

            //_server = WebApplication.Start<Starter>(port: port, verbosity: 1);

            var context = new StartContext
            {
                Parameters = parameters,
            };

            var settings = new KatanaSettings
            {
                LoaderFactory = () => (s => builder => {
                    var host = new FubuOwinHost(_runtime.Routes);
                    builder.Run(host);
                }),

            };

            var engine  = new KatanaEngine(settings);
            _server = engine.Start(context);

            _baseAddress = "http://localhost:" + port;

            _urls = _runtime.Factory.Get<IUrlRegistry>();
            _urls.As<UrlRegistry>().RootAt(_baseAddress);

            UrlContext.Stub(_baseAddress);

            _services = _runtime.Factory.Get<IServiceLocator>();
            _endpoints = new EndpointDriver(_urls);
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
            _server.Dispose();
        }

        public string BaseAddress
        {
            get { return _baseAddress; }
        }
    }
}