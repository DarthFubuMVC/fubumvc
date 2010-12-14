using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using FubuCore;
using FubuCore.Reflection;

namespace FubuMVC.Core.Packaging
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SkipOverForProvenanceAttribute : Attribute
    {
        
    }

    public static class PackageRegistry
    {
        private static readonly IList<Assembly> _assemblies = new List<Assembly>();

        static PackageRegistry()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (s, args) => _assemblies.FirstOrDefault(assembly =>
            {
                return args.Name == assembly.GetName().Name || args.Name == assembly.GetName().FullName;
            });
        }

        public static IEnumerable<Assembly> PackageAssemblies
        {
            get
            {
                return _assemblies;
            }
        }

        public static void LoadPackages(Action<IPackageFacility> configuration)
        {
            var facility = new PackageFacility();
            Diagnostics = new PackagingDiagnostics();
            var assemblyLoader = new AssemblyLoader(Diagnostics);
            var graph = new PackagingRuntimeGraph(Diagnostics, assemblyLoader);

            var codeLocation = findCallToLoadPackages();
            graph.PushProvenance(codeLocation);
            configuration(facility);
            facility.As<IPackagingRuntimeGraphConfigurer>().Configure(graph);

            graph.PopProvenance();

            graph.DiscoverAndLoadPackages(() =>
            {
                _assemblies.Clear();
                _assemblies.AddRange(assemblyLoader.Assemblies);
            });
        }

        public static PackagingDiagnostics Diagnostics { get; private set; }

        private static string findCallToLoadPackages()
        {
            var packageAssembly = typeof (IPackageInfo).Assembly;
            var trace = new StackTrace(Thread.CurrentThread, false);
            for (var i = 0; i < trace.FrameCount; i++)
            {
                var frame = trace.GetFrame(i);
                var assembly = frame.GetMethod().DeclaringType.Assembly;
                if (assembly != packageAssembly && !frame.GetMethod().HasAttribute<SkipOverForProvenanceAttribute>())
                {
                    return frame.ToDescription();
                }
            }


            return "Unknown";
        }
    }

    public static class StackFrameExtensions
    {
        public static string ToDescription(this StackFrame frame)
        {
            return "{0}.{1}(), {2} line {3}".ToFormat(frame.GetMethod().DeclaringType.FullName, frame.GetMethod().Name,
                                                      frame.GetFileName(), frame.GetFileLineNumber());
        }
    }
}