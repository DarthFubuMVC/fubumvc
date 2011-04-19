using System.Collections.Generic;
using Bottles.Deployment.Diagnostics;

namespace Bottles.Deployment.Runtime
{
    public class FinalizerSet<T> : IFinalizerSet where T : IDirective
    {
        private readonly IDirective _directive;
        private readonly ILogger _logger;
        private readonly IEnumerable<IFinalizer<T>> _finalizers;

        public FinalizerSet(IDirective directive, ILogger logger, IEnumerable<IFinalizer<T>> finalizers)
        {
            _directive = directive;
            _logger = logger;
            _finalizers = finalizers;
        }

        public void Finish()
        {
            foreach (var finalizer in _finalizers)
            {
                var name = finalizer.GetType().Name;
                var fin = finalizer;
                _logger.Log(name, ()=> fin.Finish(_directive));
            }
        }
    }
}