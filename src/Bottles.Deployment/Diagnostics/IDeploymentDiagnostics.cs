using System;
using Bottles.Diagnostics;

namespace Bottles.Deployment.Diagnostics
{
    public interface IDeploymentDiagnostics
    {
        PackageLog LogFor(object target);

        void LogHost(HostManifest hostManifest);

        void LogInitializer(IInitializer initializer, HostManifest host, Action<IInitializer> action);
        void LogDeployer(IDeployer deployer, HostManifest host, Action<IDeployer> action);
        void LogFinalizer(IFinalizer finalizer, HostManifest host, Action<IFinalizer> action);
        

        void LogDirective(IDirective directive, HostManifest host);

        //inside of the action
        void LogDeployment(IDeployer deployer, IDirective directive);
        void LogInitialization(IInitializer initializer, IDirective directive);
        void LogFinalization(IFinalizer finalizer, IDirective directive);

        void LogExecution(object target, string description, Action continuation);



        void ForEach(Action<IPackageLog> action);
    }
}