using System;
using FubuCore.Util;

namespace Bottles.Deployment.Diagnostics
{
    public class DeploymentDiagnostics : IDeploymentDiagnostics
    {
        private readonly Cache<object, PackageDeploymentLog> _logs = new Cache<object, PackageDeploymentLog>(o => new PackageDeploymentLog()
        {
            Description = o.ToString()
        });

        public void LogObject(object target, string provenance)
        {
            _logs[target].Provenance = provenance;
        }
        public PackageDeploymentLog LogFor(object target)
        {
            return _logs[target];
        }

        public void Log(string providence, Action action)
        {
            //do something
        }

        public void LogHost(HostManifest hostManifest, Action<HostManifest> action)
        {
            LogObject(hostManifest, "Host Manifest");
            LogFor(hostManifest).Description = "Host Found";
        }

        public void LogDirective(IDirective directive)
        {
            LogObject(directive, "prov");
        }

        public void LogDeployer(IDeployer deployer, Action<IDeployer> action)
        {
            LogObject(deployer, "Deployer");
            LogFor("host|directive").AddChild(deployer);
        }

        public void LogFinalizer(IFinalizer finalizer, Action<IFinalizer> action)
        {
            LogObject(finalizer, "prov");
            LogFor("host|directive").AddChild(finalizer);
        }

        public void LogInitializer(IInitializer initializer, Action<IInitializer> action)
        {
            LogObject(initializer, "prov");
            LogFor("host|directive").AddChild(initializer);
        }
    }
}