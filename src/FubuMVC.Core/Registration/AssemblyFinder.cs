using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.Core.Registration
{
    public static class AssemblyFinder
    {
        public static IEnumerable<Assembly> FindModuleAssemblies(IActivationDiagnostics diagnostics)
        {
            return FindAssemblies(file =>
            {
                diagnostics.LogFor(typeof(FubuRuntime)).Trace("Unable to load assembly from file " + file);
            }).Where(x => x.HasAttribute<FubuModuleAttribute>()).ToArray();
        }

        public static IEnumerable<Assembly> FindDependentAssemblies()
        {
            return FindAssemblies(file => { }).Where(x => x.GetReferencedAssemblies().Any(assem => assem.Name == "FubuMVC.Core"));
        }

        public static IEnumerable<Assembly> FindAssemblies(Action<string> logFailure)
        {
            var assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
            var binPath = FindBinPath();
            if (StringExtensions.IsNotEmpty(binPath))
            {
                assemblyPath = assemblyPath.AppendPath(binPath);
            }


            var files = new FileSystem().FindFiles(assemblyPath, FileSet.Deep("*.dll;*.exe"));
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
                    logFailure(file);
                }

                if (assembly != null) yield return assembly;
            }
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


        public static IEnumerable<Assembly> FindAssemblies(Func<Assembly, bool> filter,
            Action<string> onDirectoryFound = null)
        {
            if (onDirectoryFound == null)
            {
                onDirectoryFound = dir => { };
            }

            return FindAssemblies(file => { }).Where(filter);
        }

        /// <summary>
        ///   Finds the currently executing assembly.
        /// </summary>
        /// <returns></returns>
        public static Assembly FindTheCallingAssembly()
        {
            var trace = new StackTrace(false);

            var thisAssembly = Assembly.GetExecutingAssembly().GetName().Name;
            var fubuCore = typeof (IObjectResolver).Assembly.GetName().Name;

            Assembly callingAssembly = null;
            for (var i = 0; i < trace.FrameCount; i++)
            {
                var frame = trace.GetFrame(i);
                var assembly = frame.GetMethod().DeclaringType.Assembly;
                var name = assembly.GetName().Name;

                if (name != thisAssembly && name != fubuCore && name != "mscorlib" &&
                    name != "FubuMVC.Katana" && name != "FubuMVC.NOWIN" && name != "Serenity" && name != "System.Core")
                {
                    callingAssembly = assembly;
                    break;
                }
            }

            return callingAssembly;
        }
    }
}