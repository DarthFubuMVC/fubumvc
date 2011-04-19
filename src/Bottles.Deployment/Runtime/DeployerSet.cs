using System.Collections.Generic;
using Bottles.Deployment.Diagnostics;

namespace Bottles.Deployment.Runtime
{
    public class DeployerSet<T> : IDeployerSet where T : IDirective
    {
        private readonly IDirective _directive;
        private readonly ILogger _logger;
        private readonly IEnumerable<IDeployer<T>> _deployers;

        public DeployerSet(IDirective directive, ILogger logger, IEnumerable<IDeployer<T>> deployers)
        {
            _directive = directive;
            _logger = logger;
            _deployers = deployers;
        }

        public void Deploy()
        {
            foreach (var deployer in _deployers)
            {
                var name = deployer.GetType().Name;
                var dep = deployer;
                _logger.Log(name, () => dep.Deploy(_directive));
            }
        }
    }
}