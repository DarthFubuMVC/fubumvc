using System;

using Autofac;

using FubuCore.Binding;

using FubuMVC.Core.Behaviors;


namespace FubuMVC.Autofac {
	public class NestedAutofacContainerBehavior : IActionBehavior, IDisposable {
		private readonly IContainer _container;
		private readonly ServiceArguments _arguments;
		private readonly Guid _behaviorId;

		private ILifetimeScope _nestedScope;


		public NestedAutofacContainerBehavior(IContainer container, ServiceArguments arguments, Guid behaviorId) {
			_container = container;
			_arguments = arguments;
			_behaviorId = behaviorId;
		}


		public void Invoke() {
			IActionBehavior behavior = StartInnerBehavior();
			behavior.Invoke();
		}

		public IActionBehavior StartInnerBehavior() {
			_nestedScope = _container.BeginLifetimeScope(
				builder => _arguments.EachService((type, value) => builder.Register(context => value).As(type)));

			var behavior = _nestedScope.Resolve<IActionBehavior>(new PositionalParameter(0, _behaviorId.ToString()));
			return behavior;
		}

		public void InvokePartial() {
			throw new NotSupportedException();
		}

		public void Dispose() {
			_nestedScope.Dispose();
		}
	}
}