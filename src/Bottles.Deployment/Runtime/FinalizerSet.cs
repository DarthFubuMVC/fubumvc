using System.Collections.Generic;
using Bottles.Deployment.Diagnostics;

namespace Bottles.Deployment.Runtime
{
    public class FinalizerSet<T> : IDeploymentActionSet where T : IDirective
    {
        private readonly IDeploymentDiagnostics _deploymentDiagnostics;
        private readonly IEnumerable<IFinalizer<T>> _finalizers;

        public FinalizerSet(IDeploymentDiagnostics deploymentDiagnostics, IEnumerable<IFinalizer<T>> finalizers)
        {
            _deploymentDiagnostics = deploymentDiagnostics;
            _finalizers = finalizers;
        }

        public void Process(HostManifest hostManifest, IDirective directive)
        {
            //TODO: Ordering of deployers
            foreach (var finalizer in _finalizers)
            {
                _deploymentDiagnostics.LogFinalizer(finalizer, hostManifest, f =>
                {
                    f.Finish(directive);
                });
            }
        }
    }
}