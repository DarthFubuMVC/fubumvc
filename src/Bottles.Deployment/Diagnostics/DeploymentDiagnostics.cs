using System;
using FubuCore;
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
            try
            {
                action();
            }
            catch (Exception)
            {
                //what should I do here
                throw;
            }
            
        }

        public void LogDeployment(IDeployer deployer, IDirective directive)
        {
            LogObject(deployer, "Running with directive '{0}'".ToFormat(directive));
            LogFor(directive).AddChild(deployer);
        }

        public void LogHost(HostManifest hostManifest)
        {
            LogObject(hostManifest, "Deployment");
            LogFor(hostManifest).Description = hostManifest.Name;
        }

        public void LogDirective(IDirective directive, HostManifest host)
        {
            LogObject(directive, "Found in '{0}'".ToFormat(host));
            LogFor(host).AddChild(directive);
        }

        public void LogDeployer(IDeployer deployer, HostManifest host,  Action<IDeployer> action)
        {
            LogObject(deployer, "Running for host '{0}'".ToFormat(host));
            LogFor(host).AddChild(deployer);
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