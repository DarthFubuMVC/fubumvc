using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Packaging;

namespace FubuMVC.Diagnostics
{
    public class AppDomainFubuDiagnostics
    {
        private readonly AppReloaded _reloaded;

        public AppDomainFubuDiagnostics(AppReloaded reloaded)
        {
            _reloaded = reloaded;
        }

        public Dictionary<string, object> get_appdomain()
        {
            var dict = new Dictionary<string, object>
            {
                {"reloaded", _reloaded.Timestamp.ToLocalTime().ToString()},
                {"fubuPath", FubuMvcPackageFacility.GetApplicationPath()},
                {"baseDirectory", AppDomain.CurrentDomain.BaseDirectory},
                {"binPath", FubuMvcPackageFacility.FindBinPath()},
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