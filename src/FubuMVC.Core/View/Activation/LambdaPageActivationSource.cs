using System;
using System.Collections.Generic;

namespace FubuMVC.Core.View.Activation
{
    public class LambdaPageActivationSource : IPageActivationSource
    {
        private readonly Func<Type, bool> _filter;
        private readonly Func<Type, IPageActivationAction> _builder;

        public LambdaPageActivationSource(Func<Type, bool> filter, Func<Type, IPageActivationAction> builder)
        {
            _filter = filter;
            _builder = builder;
        }

        public IEnumerable<IPageActivationAction> ActionsFor(Type viewType)
        {
            if (_filter(viewType))
            {
                yield return _builder(viewType);
            }
        }
    }
}