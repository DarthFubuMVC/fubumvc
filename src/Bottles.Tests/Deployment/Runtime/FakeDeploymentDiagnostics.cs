using System;
using Bottles.Deployment;
using Bottles.Deployment.Diagnostics;

namespace Bottles.Tests.Deployment.Runtime
{
    public class FakeDeploymentDiagnostics : IDeploymentDiagnostics
    {
        public void Log(string providence, Action action)
        {
            action();
        }

        public void LogHost(HostManifest hostManifest, Action<HostManifest> action)
        {
            action(hostManifest);
        }

        public void LogDeployer(IDeployer deployer, Action<IDeployer> action)
        {
            action(deployer);
        }

        public void LogFinalizer(IFinalizer finalizer, Action<IFinalizer> action)
        {
            action(finalizer);
        }

        public void LogInitializer(IInitializer initializer, Action<IInitializer> action)
        {
            action(initializer);
        }
    }
}