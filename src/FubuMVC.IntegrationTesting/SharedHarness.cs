using System;
using System.Linq.Expressions;
using System.Threading;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Http.Scenarios;
using FubuMVC.Core.Registration;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting
{
    public static class TestHost
    {
        private static readonly Lazy<FubuRuntime> _host =
            new Lazy<FubuRuntime>(() =>
            {
                var registry = new FubuRegistry();
                registry.Features.Diagnostics.Enable(TraceLevel.Verbose);

                return registry.ToRuntime();
            });

        public static ManualResetEvent Finish = new ManualResetEvent(false);

        public static T Service<T>()
        {
            return _host.Value.Get<T>();
        }

        public static void Scenario(Action<Scenario> configuration)
        {
            _host.Value.Scenario(configuration);
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

    [SetUpFixture]
    public class HarnessBootstrapper
    {
        [TearDown]
        public void TearDown()
        {
            SelfHostHarness.Shutdown();
            TestHost.Shutdown();
        }
    }

    public static class SelfHostHarness
    {
        private static FubuRuntime _host;

        public static void Start()
        {
            Recycle();
        }

        public static string GetRootDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory.ParentDirectory().ParentDirectory();
        }


        public static FubuRuntime Host
        {
            get
            {
                if (_host == null) Recycle();

                return _host;
            }
        }

        public static string Root
        {
            get
            {
                if (_host == null) Recycle();

                return _host.BaseAddress;
            }
        }

        public static EndpointDriver Endpoints
        {
            get
            {
                if (_host == null) Recycle();

                return _host.Endpoints;
            }
        }

        public static void Shutdown()
        {
            if (_host != null) _host.SafeDispose();
        }

        public static void Recycle()
        {
            if (_host != null)
            {
                _host.Dispose();
            }

            var runtime = bootstrapRuntime();

            _host = runtime;
        }

        private static FubuRuntime bootstrapRuntime()
        {
            var registry = new HarnessRegistry {RootPath = GetRootDirectory()};
            return registry.ToRuntime();
        }
    }

    public class HarnessRegistry : FubuRegistry
    {
        public HarnessRegistry()
        {
            HostWith<Katana>();
        }
    }

    public class QuitEndpoint
    {
        public string get_quit()
        {
            TestHost.Finish.Set();

            return "Quitting";
        }
    }
}