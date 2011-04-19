using System.Collections.Generic;
using Bottles.Deployment.Diagnostics;

namespace Bottles.Deployment.Runtime
{
    public class InitializerSet<T> : IInitializerSet where T : IDirective
    {
        private readonly IDirective _directive;
        private readonly ILogger _logger;
        private readonly IEnumerable<IInitializer<T>> _initializers;

        public InitializerSet(IDirective directive, ILogger logger, IEnumerable<IInitializer<T>> initializers)
        {
            _directive = directive;
            _logger = logger;
            _initializers = initializers;
        }

        public void Initialize()
        {
            foreach (var initializer in _initializers)
            {
                var name = initializer.GetType().Name;
                var init = initializer;
                _logger.Log(name, ()=>init.Initialize(_directive));
            }
        }
    }
}