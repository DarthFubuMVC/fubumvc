using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Routing;
using Bottles;
using Bottles.Diagnostics;
using Bottles.Services;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Configuration;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core
{
    // PLEASE NOTE:  This code is primarily tested through the integration tests

    /// <summary>
    /// Key class used to define and bootstrap a FubuMVC application
    /// </summary>
    public class FubuApplication : IContainerFacilityExpression, IApplication<FubuRuntime>
    {
        private readonly Lazy<FubuRegistry> _registry;
        private Func<IContainerFacility> _facilitySource;

        private FubuApplication(Func<FubuRegistry> registryBuilder)
        {
            _registry = new Lazy<FubuRegistry>(registryBuilder);
        }

        FubuApplication IContainerFacilityExpression.ContainerFacility(IContainerFacility facility)
        {
            return registerContainerFacilitySource(() => facility);
        }

        FubuApplication IContainerFacilityExpression.ContainerFacility(Func<IContainerFacility> facilitySource)
        {
            return registerContainerFacilitySource(facilitySource);
        }

        private FubuApplication registerContainerFacilitySource(Func<IContainerFacility> facilitySource)
        {
            _facilitySource = facilitySource;
            return this;
        }

        /// <summary>
        /// Use the policies and conventions specified by the FubuRegistry built by the specified lambda.  
        /// Use this overload if the FubuRegistry type needs to use the scanned Bottle assemblies and packages 
        /// hanging off of PackageRegistry
        /// </summary>
        /// <param name="registry"></param>
        /// <returns></returns>
        public static IContainerFacilityExpression For(Func<FubuRegistry> registry)
        {
            return new FubuApplication(registry);
        }

        /// <summary>
        /// Use the policies and conventions specified by the registry
        /// </summary>
        /// <param name="registry"></param>
        /// <returns></returns>
        public static IContainerFacilityExpression For(FubuRegistry registry)
        {
            return new FubuApplication(() => registry);
        }

        /// <summary>
        /// Use only the default FubuMVC policies and conventions
        /// </summary>
        /// <returns></returns>
        public static IContainerFacilityExpression DefaultPolicies()
        {
            var assembly = FindTheCallingAssembly();
            return new FubuApplication(() => new FubuRegistry((Assembly) assembly));
        }

        /// <summary>
        /// Use the policies and conventions specified by a specific FubuRegistry of type "T"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IContainerFacilityExpression For<T>() where T : FubuRegistry, new()
        {
            return For(() => new T());
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

            var diagnostics = new BottlingDiagnostics();
            var perfTimer = diagnostics.Timer;
            perfTimer.Start("FubuMVC Application Bootstrapping");

            var packageAssemblies = perfTimer.Record("Searching for Assemblies marked with [FubuModule]", () => findModuleAssemblies(diagnostics).Where(x => x.HasAttribute<FubuModuleAttribute>()).ToArray());

            var containerFacility = _facilitySource();
            var runtime = bootstrapRuntime(perfTimer, diagnostics, packageAssemblies, containerFacility);

            var activators = runtime.Factory.GetAll<IActivator>();
            diagnostics.LogExecutionOnEachInParallel(activators, (activator, log) => activator.Activate(log));

            diagnostics.AssertNoFailures();

            diagnostics.Timer.Stop();

            Restarted = DateTime.Now;

            return runtime;
        }

        private static IEnumerable<Assembly> findModuleAssemblies(IBottlingDiagnostics diagnostics)
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


        /*

            /// <summary>
        /// Called to bootstrap and "start" a FubuMVC application 
        /// </summary>
        /// <returns></returns>
        [SkipOverForProvenance]
        public FubuRuntime Bootstrap()
        {
            SetupNamingStrategyForHttpHeaders();

            // TODO -- I think Bottles probably needs to enforce a "tell me the paths"
            // step maybe
            PackageRegistry.GetApplicationDirectory = GetApplicationPath;


            FubuRuntime runtime = null;

            PackageRegistry.LoadPackages(x =>
            {
                if (GetApplicationPath().IsNotEmpty())
                {
                    x.Loader(new FubuModuleAttributePackageLoader());
                }


                x.Bootstrap("FubuMVC Bootstrapping", log =>
                {
                    var diagnostics = PackageRegistry.Diagnostics;
                    var packageAssemblies = PackageRegistry.PackageAssemblies;
                    var perfTimer = PackageRegistry.Timer;

                    // container facility has to be spun up here
                    var containerFacility = _facilitySource();

                    runtime = bootstrapRuntime(perfTimer, diagnostics, packageAssemblies, containerFacility);

                    return runtime.Factory.GetAll<IActivator>();
                });
            });

            // TODO -- put this on FubuRuntime or BehaviorGraph
            Restarted = DateTime.Now;

            PackageRegistry.AssertNoFailures(
                () => { throw new FubuException(0, FubuApplicationDescriber.WriteDescription(runtime.Behaviors.Diagnostics)); });


            return runtime;
        }
         * 
         */

        private FubuRuntime bootstrapRuntime(IPerfTimer perfTimer, IBottlingDiagnostics diagnostics,
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

            containerFacility.Register(typeof (FubuRuntime), ObjectDef.ForValue(runtime));
            return runtime;
        }

        public static void SetupNamingStrategyForHttpHeaders()
        {
            BindingContext.AddNamingStrategy(HttpRequestHeaders.HeaderDictionaryNameForProperty);
        }

        private void bakeBehaviorGraphIntoContainer(BehaviorGraph graph, IContainerFacility containerFacility)
        {
            graph.As<IRegisterable>().Register(containerFacility.Register);
        }

        private BehaviorGraph buildBehaviorGraph(IPerfTimer timer, IEnumerable<Assembly> assemblies, IBottlingDiagnostics diagnostics)
        {
            var graph = BehaviorGraphBuilder.Build(_registry.Value, timer, assemblies, diagnostics);

            return graph;
        }

        // Build route objects from route definitions on graph + add packaging routes
        private IList<RouteBase> buildRoutes(IServiceFactory factory, BehaviorGraph graph)
        {
            var routes = new List<RouteBase>();

            graph.RoutePolicy.BuildRoutes(graph, factory).Each(routes.Add);

            return routes;
        }

        private void applyFubuExtensionsFromPackages(IBottlingDiagnostics diagnostics, IEnumerable<Assembly> packageAssemblies)
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
            var fubuCore = typeof(ITypeResolver).Assembly.GetName().Name;
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
        public static void AssertNoFailures(this IBottlingDiagnostics diagnostics, Action failure)
        {
            if (diagnostics.HasErrors())
            {
                failure();
            }
        }

        /// <summary>
        /// Default AssertNoFailures
        /// </summary>
        public static void AssertNoFailures(this IBottlingDiagnostics diagnostics)
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