using System;
using System.Linq.Expressions;
using System.Threading;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Http.Scenarios;
using FubuMVC.Core.Registration;
using FubuMVC.IntegrationTesting.Conneg;

namespace FubuMVC.IntegrationTesting
{
    public static class TestHost
    {
        private static readonly Lazy<FubuRuntime> _host =
            new Lazy<FubuRuntime>(() =>
            {
                var registry = new FubuRegistry();

                registry.Features.Diagnostics.Enable(TraceLevel.Verbose);

                registry.Services.AddService<IPropertyBinder, TheAnswerBinder>();

                return registry.ToRuntime();
            });

        public static FubuRuntime Runtime
        {
            get { return _host.Value; }
        }

        public static ManualResetEvent Finish = new ManualResetEvent(false);

        public static T Service<T>()
        {
            return _host.Value.Get<T>();
        }

        public static OwinHttpResponse Scenario(Action<Scenario> configuration)
        {
            return _host.Value.Scenario(configuration);
        }

        public static OwinHttpResponse GetByInput(object input)
        {
            OwinHttpResponse response = null;

            Scenario(x =>
            {
                x.Get.Input(input);

                response = x.Response;
            });

            return response;
        }

        public static OwinHttpResponse GetByAction<T>(Expression<Action<T>> expression)
        {
            OwinHttpResponse response = null;

            Scenario(x =>
            {
                x.Get.Action(expression);

                response = x.Response;
            });

            return response;
        }

        public static BehaviorGraph BehaviorGraph
        {
            get { return _host.Value.Behaviors; }
        }

        public static void Scenario<T>(Action<Scenario> configuration) where T : FubuRegistry, new()
        {
            using (var host = FubuRuntime.For<T>())
            {
                host.Scenario(configuration);
            }
        }

        public static void Shutdown()
        {
            if (_host.IsValueCreated)
            {
                _host.Value.SafeDispose();
            }
        }
    }
}