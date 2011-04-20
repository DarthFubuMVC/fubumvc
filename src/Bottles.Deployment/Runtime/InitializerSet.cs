using System.Collections.Generic;
using Bottles.Deployment.Diagnostics;

namespace Bottles.Deployment.Runtime
{
    public class InitializerSet<T> : IDeploymentActionSet where T : IDirective
    {
        
        private readonly ILogger _logger;
        private readonly IEnumerable<IInitializer<T>> _initializers;

        public InitializerSet(ILogger logger, IEnumerable<IInitializer<T>> initializers)
        {
            _logger = logger;
            _initializers = initializers;
        }

        public void DeployWith(IDirective directive)
        {
            //TODO: ordering
            foreach (var initializer in _initializers)
            {
                _logger.LogInitializer(initializer, i =>
                {
                    i.Initialize(directive);
                });
            }
        }
    }
}