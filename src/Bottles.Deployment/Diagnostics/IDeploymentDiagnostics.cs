using System;

namespace Bottles.Deployment.Diagnostics
{
    public interface IDeploymentDiagnostics
    {
        void Log(string providence, Action action);

        void LogHost(HostManifest hostManifest, Action<HostManifest> action);
        void LogDeployer(IDeployer deployer, Action<IDeployer> action);
        void LogFinalizer(IFinalizer finalizer, Action<IFinalizer> action);
        void LogInitializer(IInitializer initializer, Action<IInitializer> action);
    }
}