using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using FubuCore;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Http.Hosting
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
        private readonly IHost _host;


        public static string TryToGuessApplicationPath(Type type)
        {
            var solutionFolder =
                AppDomain.CurrentDomain.BaseDirectory.ParentDirectory().ParentDirectory().ParentDirectory();
            var applicationFolder = solutionFolder.AppendPath(type.Assembly.GetName().Name);

            if (Directory.Exists(applicationFolder)) return applicationFolder;

            return null;
        }


        public EmbeddedFubuMvcServer(FubuRuntime runtime, IHost host)
        {
            _runtime = runtime;
            _services = _runtime.Factory;
            _host = host;

            startAllNew(runtime);


            buildEndpointDriver();
        }


        private void startAllNew(FubuRuntime runtime)
        {
            startServer(runtime.Factory.Get<OwinSettings>(), runtime.Port);

            _urls = _runtime.Factory.Get<IUrlRegistry>();
            _services = _runtime.Factory.Get<IServiceFactory>();

            buildEndpointDriver();
        }


        private void buildEndpointDriver()
        {
            _baseAddress = "http://localhost:" + _runtime.Port;
            _endpoints = new EndpointDriver(_urls, _baseAddress);
        }


        private void startServer(OwinSettings settings, int port)
        {
            var appfunc = FubuOwinHost.ToAppFunc(_runtime, settings);
            settings.EnvironmentData[OwinConstants.AppMode] = _runtime.Mode == null ? string.Empty : _runtime.Mode.ToLower();
            var options = settings.EnvironmentData.ToDictionary();


            _server = _host.Start(port, appfunc, options);
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
            get { return Runtime.Files.RootPath; }
        }

        public void Dispose()
        {
            if (_runtime != null) _runtime.As<IDisposable>().Dispose();
            _server.Dispose();
        }

        public string BaseAddress
        {
            get { return _baseAddress; }
        }
    }

    [Serializable]
    public class KatanaRightsException : Exception
    {
        public KatanaRightsException(Exception innerException) : base(string.Empty, innerException)
        {
        }

        protected KatanaRightsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override string Message
        {
            get { return @"
To use Katana hosting, you need to either run with administrative rights
or optionally, use 'netsh http add urlacl url=http://+:80/MyUri user=DOMAIN\\user' at the command line.
See http://msdn.microsoft.com/en-us/library/ms733768.aspx for more information.
".Trim(); }
        }
    }
}