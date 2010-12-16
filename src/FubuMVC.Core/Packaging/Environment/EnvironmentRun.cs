using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Packaging.Environment
{
    [Serializable]
    public class EnvironmentRun
    {
        // folder where the app is. Mandatory!!
        public string ApplicationBase { get; set; }

        // Use to scan for an environment class if not there
        public string AssemblyName { get; set; }

        // Figure this out for yourself if not given
        public string EnvironmentClassName { get; set; }

        // Use web.config if it exists?
        public string ConfigurationFile { get; set; }

        public AppDomainSetup BuildAppDomainSetup()
        {
            return new AppDomainSetup{
                ApplicationBase = ApplicationBase,
                ConfigurationFile = ConfigurationFile,
                ApplicationName = "Bottles-Environment-Installation" + Guid.NewGuid(),
                ShadowCopyFiles = "true"
            };
        }


        public Type FindEnvironmentType()
        {
            if (EnvironmentClassName.IsNotEmpty())
            {
                return Type.GetType(EnvironmentClassName);
            }

            return AppDomain.CurrentDomain
                .Load(AssemblyName)
                .GetExportedTypes()
                .First(x => TypeExtensions.CanBeCastTo<IEnvironment>(x));
        }

        public void AssertIsValid()
        {
            var messages = new List<string>();
            if (ApplicationBase.IsEmpty())
            {
                messages.Add("ApplicationBase must be a valid folder");
            }

            if (EnvironmentClassName.IsEmpty() && AssemblyName.IsEmpty())
            {
                messages.Add("Either EnvironmentClassName or AssemblyName must be set");
            }

            if (messages.Any())
            {
                throw new EnvironmentRunnerException(messages.Join("; "));
            }
        }
    }
}