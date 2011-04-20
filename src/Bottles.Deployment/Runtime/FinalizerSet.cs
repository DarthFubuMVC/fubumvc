using System.Collections.Generic;
using Bottles.Deployment.Diagnostics;

namespace Bottles.Deployment.Runtime
{
    public class FinalizerSet<T> : IDeploymentActionSet where T : IDirective
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IFinalizer<T>> _finalizers;

        public FinalizerSet(ILogger logger, IEnumerable<IFinalizer<T>> finalizers)
        {
            _logger = logger;
            _finalizers = finalizers;
        }

        public void DeployWith(IDirective directive)
        {
            //TODO: Ordering of deployers
            foreach (var finalizer in _finalizers)
            {
                _logger.LogFinalizer(finalizer, f =>
                {
                    f.Finish(directive);
                });
            }
        }
    }
}