using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Services;

namespace FubuMVC.Core.Diagnostics
{
    public class AppDomainFubuDiagnostics
    {
        private readonly AppReloaded _reloaded;
        private readonly IFubuApplicationFiles _files;

        public AppDomainFubuDiagnostics(AppReloaded reloaded, IFubuApplicationFiles files)
        {
            _reloaded = reloaded;
            _files = files;
        }

        public Dictionary<string, object> get_appdomain()
        {
            var dict = new Dictionary<string, object>
            {
                {"reloaded", _reloaded.Timestamp.ToLocalTime().ToString()},
                {"fubuPath", _files.RootPath},
                {"baseDirectory", AppDomain.CurrentDomain.BaseDirectory},
                {"binPath", AssemblyFinder.FindBinPath()},
                {"config", AppDomain.CurrentDomain.SetupInformation.ConfigurationFile}
            };

            var assemblyLocations = 
                AppDomain.CurrentDomain.GetAssemblies()
                    .Select(x => new AssemblyLocation(x))
                    .OrderBy(x => x.name)
                    .ToArray();

            dict.Add("assemblies", assemblyLocations);

            return dict;
        } 
    }

    public class AssemblyLocation
    {
        public AssemblyLocation()
        {
        }

        public AssemblyLocation(Assembly assembly)
        {
            name = assembly.GetName().Name;
            version = assembly.GetName().Version.ToString();
            location = findCodebase(assembly);
        }

        public string name;
        public string location;
        public string version;

        private static string findCodebase(Assembly assem)
        {
            if (assem.IsDynamic) return "(dynamic)";

            try
            {
                var file = assem.CodeBase;
                return file ?? "None";
            }
            catch (Exception)
            {
                return "None";
            }
        }
    }


}