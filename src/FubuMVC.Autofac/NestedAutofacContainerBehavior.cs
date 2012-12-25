using System;

using Autofac;

using FubuCore.Binding;

using FubuMVC.Core.Behaviors;


namespace FubuMVC.Autofac
{
    public class NestedAutofacContainerBehavior : IActionBehavior, IDisposable
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly ServiceArguments _arguments;
        private readonly Guid _behaviorId;

        private ILifetimeScope _nestedScope;


        public NestedAutofacContainerBehavior(ILifetimeScope lifetimeScope, ServiceArguments arguments, Guid behaviorId)
        {
            _lifetimeScope = lifetimeScope;
            _arguments = arguments;
            _behaviorId = behaviorId;
        }


        public void Invoke()
        {
            IActionBehavior behavior = StartInnerBehavior();
            behavior.Invoke();
        }

        public IActionBehavior StartInnerBehavior()
        {
            _nestedScope = _lifetimeScope.BeginLifetimeScope(
                builder => _arguments.EachService((type, value) => builder.Register(context => value).As(type)));

            var behavior = _nestedScope.ResolveNamed<IActionBehavior>(_behaviorId.ToString());
            return behavior;
        }

        public void InvokePartial()
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            _nestedScope.Dispose();
        }
    }
}