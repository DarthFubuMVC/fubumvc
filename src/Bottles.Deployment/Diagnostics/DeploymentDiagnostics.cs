using System;
using System.Diagnostics;
using Bottles.Deployment.Runtime;
using Bottles.Diagnostics;
using FubuCore;
using FubuCore.Util;

namespace Bottles.Deployment.Diagnostics
{
    public class DeploymentDiagnostics : LoggingSession, IDeploymentDiagnostics
    {
        public void LogInitialization(IInitializer initializer, IDirective directive)
        {
            LogObject(initializer, "Running initializer for directive '{0}'".ToFormat(directive));
            LogFor(directive).AddChild(initializer);
        }

        public void LogFinalization(IFinalizer finalizer, IDirective directive)
        {
            LogObject(finalizer, "Running finalizer for directive '{0}'".ToFormat(directive));
            LogFor(directive).AddChild(finalizer);
        }

        public void LogDeployment(IDeployer deployer, IDirective directive)
        {
            LogObject(deployer, "Running with directive '{0}'".ToFormat(directive));
            LogFor(directive).AddChild(deployer);
        }

        public void LogHost(HostManifest hostManifest)
        {
            LogObject(hostManifest, "Deploying host from deployment ???");
            LogFor("deploymentname").AddChild(hostManifest);
        }

        public void LogDirective(IDirective directive, HostManifest host)
        {
            LogObject(directive, "Found in '{0}'".ToFormat(host));
            LogFor(host).AddChild(directive);
        }

    }
}