using System;
using Bottles.Diagnostics;

namespace Bottles.Deployment.Diagnostics
{
    public interface IDeploymentDiagnostics
    {
        PackageLog LogFor(object target);

        void LogHost(HostManifest hostManifest);
        void LogDirective(HostManifest host, IDirective directive);


        PackageLog LogAction(HostManifest host, IDirective directive, object action);
    }
}