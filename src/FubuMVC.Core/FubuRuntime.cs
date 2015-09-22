using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Routing;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Logging;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Http.Scenarios;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.StructureMap;
using FubuMVC.Core.StructureMap.Settings;
using FubuMVC.Core.Urls;
using StructureMap;

namespace FubuMVC.Core
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    /// <summary>
    /// Represents a running FubuMVC application, with access to the key parts of the application
    /// </summary>
    public class FubuRuntime : IDisposable, IScenarioSupport
    {
        private readonly IServiceFactory _factory;
        private readonly IContainer _container;
        private readonly IList<RouteBase> _routes;
        private bool _disposed;
        private readonly IFubuApplicationFiles _files;
        private readonly ActivationDiagnostics _diagnostics;
        private readonly PerfTimer _perfTimer;
        private readonly Lazy<AppFunc> _appFunc;
        private IDisposable _server;
        private string _baseAddress = "http://memory"; // leave this here


        public static FubuRuntime Basic(Action<FubuRegistry> configure = null)
        {
            var assembly = AssemblyFinder.FindTheCallingAssembly();
            var registry = new FubuRegistry(assembly);
            if (configure != null)
            {
                configure(registry);
            }

            return new FubuRuntime(registry);
        }

        public static FubuRuntime BasicBus(Action<FubuRegistry> configure = null)
        {
            return Basic(r =>
            {
                if (configure != null) configure(r);
                r.ServiceBus.Enable(true);
                r.ServiceBus.EnableInMemoryTransport();
            });
        }

        public static FubuRuntime For<T>(Action<FubuRegistry> configure = null) where T : FubuRegistry, new()
        {
            var registry = new T();
            if (configure != null)
            {
                configure(registry);
            }

            return new FubuRuntime(registry);
        }

        static FubuRuntime()
        {
            BindingContext.AddNamingStrategy(HttpRequestHeaders.HeaderDictionaryNameForProperty);
        }

        public FubuRuntime(FubuRegistry registry)
        {
            _appFunc = new Lazy<AppFunc>(() => FubuOwinHost.ToAppFunc(this));

            Port = registry.Port;
            Mode = registry.Mode;

            RouteTable.Routes.Clear();

            _diagnostics = new ActivationDiagnostics();

            _perfTimer = _diagnostics.Timer;
            _perfTimer.Start("FubuRuntime Bootstrapping");


            var packageAssemblies = AssemblyFinder.FindModuleAssemblies(_diagnostics);

            var applicationPath = registry.RootPath ?? DefaultApplicationPath();
            _files = new FubuApplicationFiles(applicationPath);

            _perfTimer.Record("Applying IFubuRegistryExtension's",
                () => applyFubuExtensionsFromPackages(_diagnostics, packageAssemblies, registry));

            _container = registry.ToContainer();

            var graph = _perfTimer.Record("Building the BehaviorGraph",
                () => BehaviorGraphBuilder.Build(registry, _perfTimer, packageAssemblies, _diagnostics, _files));

            _perfTimer.Record("Registering services into the IoC Container",
                () => registry.Config.RegisterServices(Mode, _container, graph));

            _factory = new StructureMapServiceFactory(_container);

            var routeTask = _perfTimer.RecordTask("Building Routes", () =>
            {
                var routes = buildRoutes(_factory, graph);
                routes.Each(r => RouteTable.Routes.Add(r));

                return routes;
            });

            _container.Configure(_ =>
            {
                _.Policies.OnMissingFamily<SettingPolicy>();

                _.For<IFubuApplicationFiles>().Use(_files);
                _.For<IServiceLocator>().Use<StructureMapServiceLocator>();
                _.For<FubuRuntime>().Use(this);
                _.For<IServiceFactory>().Use(_factory);
            });


            Activate();

            _routes = routeTask.Result();

            Host = registry.Host;

            if (registry.Host != null)
            {
                startHosting();
            }

            _perfTimer.Stop();
            Restarted = DateTime.Now;

            _diagnostics.AssertNoFailures();
        }

        private void startHosting()
        {
            _baseAddress = "http://localhost:" + Port;

            _perfTimer.Record("Starting up the embedded host at " + _baseAddress, () =>
            {
                var settings = Get<OwinSettings>();
                settings.EnvironmentData[OwinConstants.AppMode] = Mode == null ? string.Empty : Mode.ToLower();
                var options = settings.EnvironmentData.ToDictionary();


                _server = Host.Start(Port, _appFunc.Value, options);
            });
        }

        public string BaseAddress
        {
            get { return _baseAddress; }
        }

        public IHost Host { get; private set; }

        public int Port { get; private set; }

        public string Mode { get; set; }

        // Build route objects from route definitions on graph + add packaging routes
        private IList<RouteBase> buildRoutes(IServiceFactory factory, BehaviorGraph graph)
        {
            var routes = new List<RouteBase>();

            graph.RoutePolicy.BuildRoutes(graph, factory).Each(routes.Add);

            return routes;
        }

        private void applyFubuExtensionsFromPackages(IActivationDiagnostics diagnostics,
            IEnumerable<Assembly> packageAssemblies, FubuRegistry registry)
        {
            // THIS IS NEW, ONLY ASSEMBLIES MARKED AS [FubuModule] will be scanned
            var importers = packageAssemblies.Where(a => a.HasAttribute<FubuModuleAttribute>()).Select(
                assem => Task.Factory.StartNew(() => assem.FindAllExtensions(diagnostics))).ToArray();

            Task.WaitAll(importers);

            importers.SelectMany(x => x.Result).Each(x => x.Apply(registry));
        }


        public ActivationDiagnostics ActivationDiagnostics
        {
            get { return _diagnostics; }
        }

        public IFubuApplicationFiles Files
        {
            get { return _files; }
        }

        public T Get<T>()
        {
            // This is important, do it from IServiceFactory so 
            // that it's using the current "scope" in Serenity
            // testing
            return _factory.Get<T>();
        }

        public IList<RouteBase> Routes
        {
            get { return _routes; }
        }

        public BehaviorGraph Behaviors
        {
            get { return Get<BehaviorGraph>(); }
        }

        public void Dispose()
        {
            dispose();
            GC.SuppressFinalize(this);
        }

        private void dispose()
        {
            if (_disposed) return;

            _disposed = true;

            if (_server != null) _server.SafeDispose();

            if (_factory != null)
            {
                runDeactivators();
            }


            if (_container != null) _container.Dispose();
        }

        private void runDeactivators()
        {
            var logger = _factory.Get<ILogger>();
            var deactivators = _factory.GetAll<IDeactivator>().ToArray();


            deactivators.Each(x =>
            {
                var log = ActivationDiagnostics.LogFor(x);

                try
                {
                    x.Deactivate(log);
                }
                catch (Exception e)
                {
                    logger.Error("Failed while running Deactivator", e);
                    log.MarkFailure(e);
                }
                finally
                {
                    logger.InfoMessage(() => new DeactivatorExecuted {Deactivator = x.ToString(), Log = log});
                }
            });
        }

        ~FubuRuntime()
        {
            try
            {
                dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the finalizer {0}", ex);
            }
        }

        internal void Activate()
        {
            var activators = _container.GetAllInstances<IActivator>();
            _diagnostics.LogExecutionOnEachInParallel(activators,
                (activator, log) => activator.Activate(log, _perfTimer));

            _diagnostics.AssertNoFailures();

            _diagnostics.Timer.Stop();

            Restarted = DateTime.Now;
        }

        public DateTime? Restarted { get; private set; }

        public static string DefaultApplicationPath()
        {
            return
                HostingEnvironment.ApplicationPhysicalPath ?? determineApplicationPathFromAppDomain();
        }

        private static string determineApplicationPathFromAppDomain()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
            if (basePath.EndsWith("bin"))
            {
                basePath = basePath.Substring(0, basePath.Length - 3).TrimEnd(Path.DirectorySeparatorChar);
            }

            var segments = basePath.Split(Path.DirectorySeparatorChar);
            if (segments.Length > 2)
            {
                if (segments[segments.Length - 2].EqualsIgnoreCase("bin"))
                {
                    return basePath.ParentDirectory().ParentDirectory();
                }
            }

            return basePath;
        }


        OwinHttpResponse IScenarioSupport.Send(OwinHttpRequest request)
        {
            // TODO -- make the wait be configurable?
            request.RewindData();

            _appFunc.Value(request.Environment).Wait(15.Seconds());

            return new OwinHttpResponse(request.Environment);
        }

        public OwinHttpResponse Scenario(Action<Scenario> configuration)
        {
            var scenario = new Scenario(this);
            using (scenario)
            {
                configuration(scenario);

                return scenario.Response;
            }
        }

        string IScenarioSupport.RootUrl
        {
            get { return "http://memory"; }
        }
    }
}