using System;
using Bottles.Deployment;
using Bottles.Deployment.Diagnostics;
using Bottles.Deployment.Runtime;
using Bottles.Diagnostics;

namespace Bottles.Tests.Deployment.Runtime
{
    public class FakeDeploymentDiagnostics : IDeploymentDiagnostics
    {
        public PackageLog LogFor(object target)
        {
            return null;
        }

        public void LogHost(HostManifest hostManifest, Action<HostManifest> action)
        {
            action(hostManifest);
        }


        public void LogHost(HostManifest hostManifest)
        {
            
        }

        public void LogDirective(HostManifest host, IDirective directive)
        {
            
        }

        public PackageLog LogAction(HostManifest host, IDirective directive, object action)
        {
            throw new NotImplementedException();
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