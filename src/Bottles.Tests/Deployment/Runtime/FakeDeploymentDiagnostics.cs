using System;
using Bottles.Deployment;
using Bottles.Deployment.Diagnostics;
using Bottles.Diagnostics;

namespace Bottles.Tests.Deployment.Runtime
{
    public class FakeDeploymentDiagnostics : IDeploymentDiagnostics
    {
        public void LogHost(HostManifest hostManifest, Action<HostManifest> action)
        {
            action(hostManifest);
        }

        public void LogDeployer(IDeployer deployer, HostManifest host,  Action<IDeployer> action)
        {
            action(deployer);
        }

        public void LogFinalizer(IFinalizer finalizer, HostManifest host, Action<IFinalizer> action)
        {
            action(finalizer);
        }

        public void LogDeployment(IDeployer deployer, IDirective directive)
        {
            
        }

        public void LogHost(HostManifest hostManifest)
        {
            
        }

        public void LogDirective(IDirective directive, HostManifest host)
        {
            
        }

        public void LogInitialization(IInitializer initializer, IDirective directive)
        {
            
        }

        public void LogFinalization(IFinalizer finalizer, IDirective directive)
        {
            
        }

        public void LogInitializer(IInitializer initializer, HostManifest host, Action<IInitializer> action)
        {
            action(initializer);
        }

        public void LogExecution(object target, string description, Action continuation)
        {
            continuation();
        }

        public void ForEach(Action<IPackageLog> action)
        {
            
        }
    }
}