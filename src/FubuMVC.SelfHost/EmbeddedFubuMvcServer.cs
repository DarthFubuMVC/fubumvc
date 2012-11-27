using System;
using System.IO;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Urls;

namespace FubuMVC.SelfHost
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
            return new EmbeddedFubuMvcServer(application, physicalPath, port);
        }
    }

    /// <summary>
    /// Embeds and runs a FubuMVC application in a normal process using the Web API self host libraries
    /// </summary>
    public class EmbeddedFubuMvcServer : IDisposable
    {
        private readonly FubuApplication _application;
        private readonly string _physicalPath;
        private readonly FubuRuntime _runtime;
        private readonly SelfHostHttpServer _server;
        private readonly IUrlRegistry _urls;
        private readonly IServiceLocator _services;
        private readonly EndpointDriver _endpoints;

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

            return new EmbeddedFubuMvcServer(new T().BuildApplication(), physicalPath, port);

        }

        public static string TryToGuessApplicationPath(Type type)
        {
            var solutionFolder = AppDomain.CurrentDomain.BaseDirectory.ParentDirectory().ParentDirectory().ParentDirectory();
            var applicationFolder = solutionFolder.AppendPath(type.Assembly.GetName().Name);

            if (Directory.Exists(applicationFolder)) return applicationFolder;

            return null;
        }

        public EmbeddedFubuMvcServer(FubuApplication application, string physicalPath = null, int port = 5500)
        {
            _application = application;
            _physicalPath = physicalPath ?? AppDomain.CurrentDomain.BaseDirectory;

            _server = new SelfHostHttpServer(port, physicalPath);
            _runtime = _application.Bootstrap();
            _server.Start(_runtime);

            _urls = _runtime.Factory.Get<IUrlRegistry>();
            _urls.As<UrlRegistry>().RootAt(_server.BaseAddress);

            UrlContext.Stub(_server.BaseAddress);

            _services = _runtime.Factory.Get<IServiceLocator>();
            _endpoints = new EndpointDriver(_urls);
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
            get { return _physicalPath; }
        }

        public void Dispose()
        {
            _server.Dispose();
        }

        public string BaseAddress
        {
            get { return _server.BaseAddress; }
        }
    }
}