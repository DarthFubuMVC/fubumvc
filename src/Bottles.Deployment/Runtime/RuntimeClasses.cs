using System;
using Bottles.Diagnostics;

namespace Bottles.Deployment.Runtime
{
    public interface IDeploymentAction<T> where T : IDirective
    {
        void Execute(T directive, HostManifest host, IPackageLog log);
    }

    public interface IInitializer<T> : IDeploymentAction<T> where T : IDirective
    {

    }


    public interface IFinalizer<T> : IDeploymentAction<T> where T : IDirective
    {

    }

    public interface IDeployer<T> : IDeploymentAction<T> where T : IDirective
    {

    }
}