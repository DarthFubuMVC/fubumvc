using System;

namespace Bottles.Deployment.Diagnostics
{
    public interface ILogger
    {
        void Log(string providence, Action action);
        
        void LogDeployer(IDeployer deployer, Action<IDeployer> action);
        void LogFinalizer(IFinalizer finalizer, Action<IFinalizer> action);
        void LogInitializer(IInitializer initializer, Action<IInitializer> action);

        void LogHost(HostManifest hostManifest, Action<HostManifest> action);
        
    }
}