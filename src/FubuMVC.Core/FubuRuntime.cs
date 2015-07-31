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
using FubuCore.Util;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Services;
using FubuMVC.Core.StructureMap;
using FubuMVC.Core.StructureMap.Settings;
using StructureMap;

namespace FubuMVC.Core
{
    /// <summary>
    /// Represents a running FubuMVC application, with access to the key parts of the application
    /// </summary>
    public class FubuRuntime : IDisposable
    {
        private readonly IServiceFactory _factory;
        private readonly IContainer _container;
        private readonly IList<RouteBase> _routes;
        private bool _disposed;
        private readonly IFubuApplicationFiles _files;
        private readonly ActivationDiagnostics _diagnostics;
        private readonly PerfTimer _perfTimer;


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
            RouteTable.Routes.Clear();

            _diagnostics = new ActivationDiagnostics();

            _perfTimer = _diagnostics.Timer;
            _perfTimer.Start("FubuRuntime Bootstrapping");


            var packageAssemblies = FubuModuleFinder.FindModuleAssemblies(_diagnostics);

            var applicationPath = registry.RootPath ?? DefaultApplicationPath();
            _files = new FubuApplicationFiles(applicationPath);

            _perfTimer.Record("Applying IFubuRegistryExtension's",
                () => applyFubuExtensionsFromPackages(_diagnostics, packageAssemblies, registry));

            _container = registry.ToContainer();

            var graph = _perfTimer.Record("Building the BehaviorGraph",
                () => BehaviorGraphBuilder.Build(registry, _perfTimer, packageAssemblies, _diagnostics, _files));

            _perfTimer.Record("Registering services into the IoC Container",
                () => registry.Config.RegisterServices(_container, graph));

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

            _perfTimer.Stop();
            Restarted = DateTime.Now;

            _diagnostics.AssertNoFailures();
        }

        // Build route objects from route definitions on graph + add packaging routes
        private IList<RouteBase> buildRoutes(IServiceFactory factory, BehaviorGraph graph)
        {
            var routes = new List<RouteBase>();

            graph.RoutePolicy.BuildRoutes(graph, factory).Each(routes.Add);

            return routes;
        }

        private void applyFubuExtensionsFromPackages(IActivationDiagnostics diagnostics, IEnumerable<Assembly> packageAssemblies, FubuRegistry registry)
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


        public IContainer Container
        {
            get { return _container; }
        }

        public IServiceFactory Factory
        {
            get { return _factory; }
        }

        public IList<RouteBase> Routes
        {
            get { return _routes; }
        }

        public BehaviorGraph Behaviors
        {
            get { return Factory.Get<BehaviorGraph>(); }
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

            var logger = _factory.Get<ILogger>();
            var deactivators = _factory.GetAll<IDeactivator>().ToArray();


            deactivators.Each(x =>
            {
                var log = Behaviors.Diagnostics.LogFor(x);

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

            Container.Dispose();
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
            var activators = Container.GetAllInstances<IActivator>();
            _diagnostics.LogExecutionOnEachInParallel(activators, (activator, log) => activator.Activate(log, _perfTimer));

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

        public static readonly Cache<string, string> Properties = new Cache<string, string>(key => null);
    }
}