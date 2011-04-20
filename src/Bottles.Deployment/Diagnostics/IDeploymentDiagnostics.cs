using System;

namespace Bottles.Deployment.Diagnostics
{
    public interface IDeploymentDiagnostics
    {
        void Log(string providence, Action action);

        void LogHost(HostManifest hostManifest);
        void LogDeployer(IDeployer deployer, HostManifest host, Action<IDeployer> action);
        void LogFinalizer(IFinalizer finalizer, Action<IFinalizer> action);
        void LogInitializer(IInitializer initializer, Action<IInitializer> action);
        void LogDirective(IDirective directive, HostManifest host);
        void LogDeployment(IDeployer deployer, IDirective directive);
    }
}