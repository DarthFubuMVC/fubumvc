using System.Collections.Generic;
using Bottles.Deployment.Diagnostics;

namespace Bottles.Deployment.Runtime
{
    public class DeployerSet<T> : IDeploymentActionSet where T : IDirective
    {
        private readonly IDeploymentDiagnostics _deploymentDiagnostics;
        private readonly IEnumerable<IDeployer<T>> _deployers;

        public DeployerSet(IDeploymentDiagnostics deploymentDiagnostics, IEnumerable<IDeployer<T>> deployers)
        {
            _deploymentDiagnostics = deploymentDiagnostics;
            _deployers = deployers;
        }

        public void Process(HostManifest hostManifest, IDirective directive)
        {
            //TODO: ordering of deployers?
            foreach (var deployer in _deployers)
            {
                _deploymentDiagnostics.LogDeployer(deployer, hostManifest, d=>
                {
                    d.Deploy(directive);
                });
            }
        }
    }
}