using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Http.Owin.Middleware.StaticFiles;
using FubuMVC.Core.Http.Scenarios;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Security;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Http.Hosting
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class InMemoryHost : IDisposable
    {
        public static readonly string RootUrl = "http://memory";
        private readonly FubuRuntime _runtime;
        private readonly IServiceLocator _services;
        private readonly AppFunc _func;


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

            _func = FubuOwinHost.ToAppFunc(runtime);

            _services = _runtime.Factory.Get<IServiceLocator>();
        }

        public IServiceLocator Services
        {
            get { return _services; }
        }

        public BehaviorGraph Behaviors
        {
            get
            {
                return _services.GetInstance<BehaviorGraph>();
            }
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
            request.RewindData();

            _func(request.Environment).Wait(15.Seconds());

            return new OwinHttpResponse(request.Environment);
        }

        public OwinHttpResponse Scenario(Action<Scenario> configuration)
        {
            var scenario = CreateScenario();
            using (scenario)
            {
                configuration(scenario);

                return scenario.Response;
            }
        }

        public Scenario CreateScenario()
        {
            var request = OwinHttpRequest.ForTesting();
            request.FullUrl(RootUrl);

            var securitySettings = _services.GetInstance<SecuritySettings>();
            securitySettings.Reset();
            var scenario = new Scenario(_runtime.Factory.Get<IUrlRegistry>(), request, Send, securitySettings);
            return scenario;
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