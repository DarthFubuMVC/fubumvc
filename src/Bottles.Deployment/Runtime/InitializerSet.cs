using System.Collections.Generic;
using Bottles.Deployment.Diagnostics;

namespace Bottles.Deployment.Runtime
{
    public class InitializerSet<T> : IDeploymentActionSet where T : IDirective
    {
        
        private readonly IDeploymentDiagnostics _deploymentDiagnostics;
        private readonly IEnumerable<IInitializer<T>> _initializers;

        public InitializerSet(IDeploymentDiagnostics deploymentDiagnostics, IEnumerable<IInitializer<T>> initializers)
        {
            _deploymentDiagnostics = deploymentDiagnostics;
            _initializers = initializers;
        }

        public void Process(HostManifest hostManifest, IDirective directive)
        {
            //TODO: ordering
            foreach (var initializer in _initializers)
            {
                _deploymentDiagnostics.LogInitializer(initializer, i =>
                {
                    i.Initialize(directive);
                });
            }
        }
    }
}