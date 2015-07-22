using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Routing;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Services;
using FubuMVC.Core.StructureMap;
using StructureMap;
using StructureMap.Pipeline;

namespace FubuMVC.Core
{
    // PLEASE NOTE:  This code is primarily tested through the integration tests

    /// <summary>
    /// Key class used to define and bootstrap a FubuMVC application
    /// </summary>
    public class FubuApplication : IApplication<FubuRuntime>
    {
        private readonly Lazy<FubuRegistry> _registry;

        private FubuApplication(Func<FubuRegistry> registryBuilder)
        {
            _registry = new Lazy<FubuRegistry>(registryBuilder);
        }

        /// <summary>
        /// Use the policies and conventions specified by the FubuRegistry built by the specified lambda.  
        /// Use this overload if the FubuRegistry type needs to use the scanned Bottle assemblies and packages 
        /// hanging off of PackageRegistry
        /// </summary>
        /// <param name="registry"></param>
        /// <returns></returns>
        public static FubuApplication For(Func<FubuRegistry> registry)
        {
            return new FubuApplication(registry);
        }

        /// <summary>
        /// Use the policies and conventions specified by the registry
        /// </summary>
        /// <param name="registry"></param>
        /// <returns></returns>
        public static FubuApplication For(FubuRegistry registry)
        {
            return new FubuApplication(() => registry);
        }

        /// <summary>
        /// Use only the default FubuMVC policies and conventions
        /// </summary>
        /// <returns></returns>
        public static FubuApplication DefaultPolicies(IContainer container = null)
        {
            var assembly = FindTheCallingAssembly();
            return new FubuApplication(() =>
            {
                var registry = new FubuRegistry(assembly);
                if (container != null)
                {
                    registry.StructureMap(container);
                }


                return registry;
            });
        }

        /// <summary>
        /// Use the policies and conventions specified by a specific FubuRegistry of type "T"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static FubuApplication For<T>(Action<T> customize = null) where T : FubuRegistry, new()
        {
            return For(() =>
            {
                var registry = new T();
                if (customize != null)
                {
                    customize(registry);
                }

                return registry;
            });
        }


        /// <summary>
        /// Shortcut method to immediately bootstrap the specified FubuMVC application
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static FubuRuntime BootstrapApplication<T>() where T : IApplicationSource, new()
        {
            return new T().BuildApplication().Bootstrap();
        }

        /// <summary>
        /// Called to bootstrap and "start" a FubuMVC application 
        /// </summary>
        /// <returns></returns>
        public FubuRuntime Bootstrap()
        {
            RouteTable.Routes.Clear();
            SetupNamingStrategyForHttpHeaders();

            var diagnostics = new ActivationDiagnostics();
            var perfTimer = diagnostics.Timer;
            perfTimer.Start("FubuMVC Application Bootstrapping");

            var packageAssemblies = perfTimer.Record("Searching for Assemblies marked with [FubuModule]", () => findModuleAssemblies(diagnostics).Where(x => x.HasAttribute<FubuModuleAttribute>()).ToArray());

            // TODO -- going to remove this
            var containerFacility = new StructureMapContainerFacility(_registry.Value.ToContainer());
            var runtime = bootstrapRuntime(perfTimer, diagnostics, packageAssemblies, containerFacility);

            var activators = runtime.Factory.GetAll<IActivator>();
            diagnostics.LogExecutionOnEachInParallel(activators, (activator, log) => activator.Activate(log));

            diagnostics.AssertNoFailures();

            diagnostics.Timer.Stop();

            Restarted = DateTime.Now;

            return runtime;
        }

        private static IEnumerable<Assembly> findModuleAssemblies(IActivationDiagnostics diagnostics)
        {
            var assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
            var binPath = FindBinPath();
            if (binPath.IsNotEmpty())
            {
                assemblyPath = assemblyPath.AppendPath(binPath);
            }


            var files = new FileSystem().FindFiles(assemblyPath, FileSet.Deep("*.dll"));
            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                Assembly assembly = null;

                try
                {
                    assembly = AppDomain.CurrentDomain.Load(name);
                }
                catch (Exception)
                {
                    diagnostics.LogFor(typeof(FubuApplication)).Trace("Unable to load assembly from file " + file);
                }

                if (assembly != null) yield return assembly;
            }

        }


        private FubuRuntime bootstrapRuntime(IPerfTimer perfTimer, IActivationDiagnostics diagnostics,
            IEnumerable<Assembly> packageAssemblies, IContainerFacility containerFacility)
        {
            perfTimer.Record("Applying IFubuRegistryExtension's", () => applyFubuExtensionsFromPackages(diagnostics, packageAssemblies));

            var graph = perfTimer.Record("Building the BehaviorGraph",
                () => buildBehaviorGraph(perfTimer, packageAssemblies, diagnostics));

            perfTimer.Record("Registering services into the IoC Container",
                () => bakeBehaviorGraphIntoContainer(graph, containerFacility));

            // factory HAS to be spun up here.
            var factory = perfTimer.Record("Build the IServiceFactory",
                () => containerFacility.BuildFactory(graph));

            var routeTask = perfTimer.RecordTask("Building Routes", () =>
            {
                var routes = buildRoutes(factory, graph);
                routes.Each(r => RouteTable.Routes.Add(r));

                return routes;
            });

            var runtime = new FubuRuntime(factory, containerFacility, routeTask.Result);

            containerFacility.Register(typeof (FubuRuntime), new ObjectInstance(runtime));
            return runtime;
        }

        public static void SetupNamingStrategyForHttpHeaders()
        {
            BindingContext.AddNamingStrategy(HttpRequestHeaders.HeaderDictionaryNameForProperty);
        }

        private void bakeBehaviorGraphIntoContainer(BehaviorGraph graph, IContainerFacility containerFacility)
        {
            graph.As<IRegisterable>().Register((serviceType, def) => containerFacility.Register(serviceType, def));
            _registry.Value.Config.RegisterServices(containerFacility, graph);
        }

        private BehaviorGraph buildBehaviorGraph(IPerfTimer timer, IEnumerable<Assembly> assemblies, IActivationDiagnostics diagnostics)
        {
            return BehaviorGraphBuilder.Build(_registry.Value, timer, assemblies, diagnostics);
        }

        // Build route objects from route definitions on graph + add packaging routes
        private IList<RouteBase> buildRoutes(IServiceFactory factory, BehaviorGraph graph)
        {
            var routes = new List<RouteBase>();

            graph.RoutePolicy.BuildRoutes(graph, factory).Each(routes.Add);

            return routes;
        }

        private void applyFubuExtensionsFromPackages(IActivationDiagnostics diagnostics, IEnumerable<Assembly> packageAssemblies)
        {
            // THIS IS NEW, ONLY ASSEMBLIES MARKED AS [FubuModule] will be scanned
            var importers = packageAssemblies.Where(a => a.HasAttribute<FubuModuleAttribute>()).Select(
                assem => Task.Factory.StartNew(() => assem.FindAllExtensions(diagnostics))).ToArray();

            Task.WaitAll(importers);

            importers.SelectMany(x => x.Result).Each(x => x.Apply(_registry.Value));
        }

        public static string PhysicalRootPath { get; set; }


        public static string GetApplicationPath()
        {
            return PhysicalRootPath ??
                   HostingEnvironment.ApplicationPhysicalPath ?? determineApplicationPathFromAppDomain();
        }

        public static string FindBinPath()
        {
            var binPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
            if (binPath.IsNotEmpty())
            {
                return Path.IsPathRooted(binPath)
                    ? binPath
                    : AppDomain.CurrentDomain.SetupInformation.ApplicationBase.AppendPath(binPath);
            }

            return null;
        }


        public static DateTime? Restarted { get; set; }

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

        /// <summary>
        ///   Finds the currently executing assembly.
        /// </summary>
        /// <returns></returns>
        public static Assembly FindTheCallingAssembly()
        {
            var trace = new StackTrace(false);

            var thisAssembly = Assembly.GetExecutingAssembly().GetName().Name;
            var fubuCore = typeof(IObjectResolver).Assembly.GetName().Name;
            var bottles = typeof(IActivator).Assembly.GetName().Name;

            Assembly callingAssembly = null;
            for (int i = 0; i < trace.FrameCount; i++)
            {
                StackFrame frame = trace.GetFrame(i);
                Assembly assembly = frame.GetMethod().DeclaringType.Assembly;
                var name = assembly.GetName().Name;

                if (name != thisAssembly && name != fubuCore && name != bottles && name != "mscorlib" && name != "FubuMVC.Katana" && name != "Serenity" && name != "System.Core" && name != "FubuTransportation")
                {
                    callingAssembly = assembly;
                    break;
                }
            }
            return callingAssembly;
        }

        public readonly static Cache<string, string> Properties = new Cache<string, string>(key => null); 
    }

    public static class DiagnosticsExtensions
    {
        /// <summary>
        /// A static method that should be exposed, to allow you to 
        /// take an action when there has been a failure in the system.
        /// </summary>
        /// <param name="failure">The action to perform</param>
        public static void AssertNoFailures(this IActivationDiagnostics diagnostics, Action failure)
        {
            if (diagnostics.HasErrors())
            {
                failure();
            }
        }

        /// <summary>
        /// Default AssertNoFailures
        /// </summary>
        public static void AssertNoFailures(this IActivationDiagnostics diagnostics)
        {
            diagnostics.AssertNoFailures(() =>
            {
                var writer = new StringWriter();
                writer.WriteLine("Package loading and application bootstrapping failed");
                writer.WriteLine();
                diagnostics.EachLog((o, log) =>
                {
                    if (!log.Success)
                    {
                        writer.WriteLine(o.ToString());
                        writer.WriteLine(log.FullTraceText());
                        writer.WriteLine("------------------------------------------------------------------------------------------------");
                    }
                });

                throw new FubuException(1, writer.GetStringBuilder().ToString());
            });
        }
    }
}