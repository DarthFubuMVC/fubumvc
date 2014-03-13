using System;
using FubuCore;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Http.Scenarios;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Urls;

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

            return Send(request);
        }

        public OwinHttpResponse Send(OwinHttpRequest request)
        {
            // TODO -- make the wait be configurable?
            _host.Invoke(request.Environment).Wait(15.Seconds());

            return new OwinHttpResponse(request.Environment);
        }

        public OwinHttpResponse Scenario(Action<IScenario> configuration)
        {
            var request = OwinHttpRequest.ForTesting();
            request.FullUrl(RootUrl);

            using (var scenario = new Scenario(_runtime.Factory.Get<IUrlRegistry>(), request, Send))
            {
                configuration(scenario);

                return scenario.Response;
            }
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