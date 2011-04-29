using System;
using Bottles.Deployment;
using Bottles.Deployment.Runtime;
using Bottles.Diagnostics;

namespace Bottles.Tests.Deployment
{
    public class FakeDeployer<T> : IDeployer<T> where T : IDirective
    {

        public T PassedInDirective { get; private set; }
        public bool DeployWasCalled { get; private set; }
        public void Execute(T directive, HostManifest host, IPackageLog log)
        {
            PassedInDirective = directive;
            DeployWasCalled = true;
        }
    }
}