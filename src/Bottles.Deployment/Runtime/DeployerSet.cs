using System.Collections.Generic;
using Bottles.Deployment.Diagnostics;

namespace Bottles.Deployment.Runtime
{
    public class DeployerSet<T> : IDeploymentActionSet where T : IDirective
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IDeployer<T>> _deployers;

        public DeployerSet(ILogger logger, IEnumerable<IDeployer<T>> deployers)
        {
            _logger = logger;
            _deployers = deployers;
        }

        public void DeployWith(IDirective directive)
        {
            //TODO: ordering of deployers?
            foreach (var deployer in _deployers)
            {
                _logger.LogDeployer(deployer, d=>
                {
                    d.Deploy(directive);
                });
            }
        }
    }
}