using System;
using FubuCore;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Packaging;

namespace FubuMVC.Core.Http.Hosting
{
    public class InMemoryHost : IDisposable
    {
        public static readonly string RootUrl = "http://memory";
        private readonly FubuRuntime _runtime;
        private readonly FubuOwinHost _host;


        public static InMemoryHost For<T>(string directory = null) where T : IApplicationSource, new()
        {
            if (directory.IsNotEmpty())
            {
                FubuMvcPackageFacility.PhysicalRootPath = directory;
            }

            var runtime = new T().BuildApplication().Bootstrap();
            return new InMemoryHost(runtime);
        }

        public InMemoryHost(FubuRuntime runtime)
        {
            _runtime = runtime;

            _host = new FubuOwinHost(runtime.Routes);
        }

        public OwinHttpResponse Send(Action<OwinHttpRequest> configuration)
        {
            var request = OwinHttpRequest.ForTesting();
            request.FullUrl(RootUrl);

            configuration(request);

            // TODO -- make the wait be configurable?
             _host.Invoke(request.Environment).Wait(15.Seconds());

            return new OwinHttpResponse(request.Environment);
        }

        void IDisposable.Dispose()
        {
            _runtime.Dispose();
        }
    }

    public static class InMemoryHostExtensions
    {
        public static InMemoryHost RunInMemory(this FubuApplication application, string directory = null)
        {
            if (directory.IsNotEmpty())
            {
                FubuMvcPackageFacility.PhysicalRootPath = directory;
            }

            return new InMemoryHost(application.Bootstrap());
        }
    }
}