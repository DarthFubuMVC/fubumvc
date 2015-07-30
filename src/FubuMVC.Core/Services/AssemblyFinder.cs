using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Binding;

namespace FubuMVC.Core.Services
{
    public static class AssemblyFinder
    {
        public static IEnumerable<Assembly> FindAssemblies(Func<Assembly, bool> filter,
            Action<string> onDirectoryFound = null)
        {
            if (onDirectoryFound == null)
            {
                onDirectoryFound = dir => { };
            }

            var list = new List<string> {AppDomain.CurrentDomain.SetupInformation.ApplicationBase};

            var binPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
            if (binPath.IsNotEmpty())
            {
                if (Path.IsPathRooted(binPath))
                {
                    list.Add(binPath);
                }
                else
                {
                    list.Add(AppDomain.CurrentDomain.SetupInformation.ApplicationBase.AppendPath(binPath));
                }
            }

            list.Each(x => onDirectoryFound(x));

            return FindAssemblies(list, filter);
        }

        public static IEnumerable<Assembly> FindAssemblies(IEnumerable<string> directories, Func<Assembly, bool> filter)
        {
            return directories.SelectMany(x => AssembliesFromPath(x, filter));
        }

        public static IEnumerable<Assembly> AssembliesFromPath(string path, Func<Assembly, bool> assemblyFilter)
        {
            var assemblyPaths = Directory.GetFiles(path)
                .Where(file =>
                    Path.GetExtension(file).Equals(
                        ".exe",
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    Path.GetExtension(file).Equals(
                        ".dll",
                        StringComparison.OrdinalIgnoreCase));

            foreach (var assemblyPath in assemblyPaths)
            {
                var assembly =
                    AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(
                        x => x.GetName().Name == Path.GetFileNameWithoutExtension(assemblyPath));

                if (assembly == null)
                {
                    try
                    {
                        assembly = Assembly.LoadFrom(assemblyPath);
                    }
                    catch
                    {
                    }
                }


                if (assembly != null && assemblyFilter(assembly))
                {
                    yield return assembly;
                }
            }
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