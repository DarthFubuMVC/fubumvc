using System;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.View.Activation
{
    public class LambdaPageActivationAction : IPageActivationAction
    {
        private readonly Action<IServiceLocator, IFubuPage> _action;

        public LambdaPageActivationAction(Action<IServiceLocator, IFubuPage> action)
        {
            _action = action;
        }

        public LambdaPageActivationAction(Action<IFubuPage> action) : this((s, p) => action(p))
        {
        }

        public void Activate(IServiceLocator services, IFubuPage page)
        {
            _action(services, page);
        }
    }
}