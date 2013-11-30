using System;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Windsor
{
    public class NestedWindsorContainerBehavior : IActionBehavior, IDisposable
    {
        private readonly ServiceArguments _arguments;
        private readonly Guid _behaviorId;
        private readonly IWindsorContainer _windsorContainer;
        private IDisposable _scope;

        public NestedWindsorContainerBehavior(IWindsorContainer windsorContainer, ServiceArguments arguments,
                                              Guid behaviorId)
        {
            _windsorContainer = windsorContainer;
            _arguments = arguments;
            _behaviorId = behaviorId;
        }

        public void Invoke()
        {
            var behavior = StartInnerBehavior();
            behavior.Invoke();
        }

        public void InvokePartial()
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            _scope.Dispose();
            
        }

        public IActionBehavior StartInnerBehavior()
        {
            _scope = _windsorContainer.BeginScope();

            _arguments.EachService((type, value) => _windsorContainer.Register(Component.For(type).Instance(value).LifestyleScoped()));

            return _windsorContainer.Resolve<IActionBehavior>(_behaviorId.ToString());
        }
    }
}