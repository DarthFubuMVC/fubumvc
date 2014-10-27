using System;
using System.Linq.Expressions;
using System.Threading;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Http.Scenarios;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using NUnit.Framework;
using StructureMap;
using Process = System.Diagnostics.Process;

namespace FubuMVC.IntegrationTesting
{
    public static class TestHost
    {
        private static readonly Lazy<InMemoryHost> _host =
            new Lazy<InMemoryHost>(() => { return FubuApplication.DefaultPolicies().StructureMap().RunInMemory(); });

        public static ManualResetEvent Finish = new ManualResetEvent(false);

        public static void Scenario(Action<Scenario> configuration)
        {
            _host.Value.Scenario(configuration);
        }

        public static OwinHttpResponse GetByInput(object input)
        {
            OwinHttpResponse response = null;

            Scenario(x => {
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
            get
            {
                return _host.Value.Behaviors;
            }
        }

        public static void Scenario<T>(Action<Scenario> configuration) where T : FubuRegistry, new()
        {
            using (var host = FubuApplication.For<T>().StructureMap().RunInMemory())
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
        private static EmbeddedFubuMvcServer _server;
        private static InMemoryHost _host;

        public static void Start()
        {
            Recycle();
        }

        public static string GetRootDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory.ParentDirectory().ParentDirectory();
        }

        public static EmbeddedFubuMvcServer Server
        {
            get
            {
                if (_server == null) Recycle();
                
                return _server;
            }
        }

        public static InMemoryHost Host
        {
            get
            {
                if (_server == null) Recycle();
                
                return _host;
            }
        }

        public static string Root
        {
            get
            {
                if (_server == null) Recycle();

                return _server.BaseAddress;
            }
        }

        public static EndpointDriver Endpoints
        {
            get
            {
                if (_server == null) Recycle();

                return _server.Endpoints;
            }
        }

        public static void Shutdown()
        {
            if (_server != null) _server.SafeDispose();
        }

        public static void Recycle()
        {
            if (_server != null)
            {
                _server.Dispose();
            }

            var port = PortFinder.FindPort(5500);
            var runtime = bootstrapRuntime();

            _server = new EmbeddedFubuMvcServer(runtime, GetRootDirectory(), port);
            _host = new InMemoryHost(runtime);
        }

        private static FubuRuntime bootstrapRuntime()
        {
            return FubuApplication.For<HarnessRegistry>().StructureMap(new Container()).Bootstrap();
        }
    }

    public class HarnessRegistry : FubuRegistry
    {
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