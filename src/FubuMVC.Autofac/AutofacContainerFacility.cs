using System;
using System.Collections.Generic;

using Autofac;
using Autofac.Features.ResolveAnything;

using FubuCore.Binding;

using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;


namespace FubuMVC.Autofac {
	public class AutofacContainerFacility : IContainerFacility, IServiceFactory {
		private readonly IComponentContext _context;
		private readonly AutofacFubuModule _module;

		private Action<Type, ObjectDef> _register;


		public AutofacContainerFacility(IComponentContext context) {
			_context = context;
			_module = new AutofacFubuModule();

			_register = (serviceType, def) => {
				bool isSingleton = (ServiceRegistry.ShouldBeSingleton(serviceType) || ServiceRegistry.ShouldBeSingleton(def.Type) || def.IsSingleton);

				if (def.Value == null) {
					// TODO: Account for def.Value == null.
					_module.AddTypeRegistration(serviceType, def.Type, def.Name, isSingleton);
				} else {
					_module.AddInstanceRegistration(serviceType, def.Value, def.Name, isSingleton);
				}
			};
		}


		public IComponentContext Context {
			get { return _context; }
		}


		public IServiceFactory BuildFactory() {
			var builder = GetContainerBuilder();
			builder.RegisterModule(_module);
			builder.Update(_context.ComponentRegistry);

			_register = (serviceType, def) => {
				if (def.Value == null) {
					// TODO: Account for def.Value == null.
					RegisterType(serviceType, def.Type);
				} else {
					RegisterInstance(serviceType, def.Value);
				}
			};

			return this;
		}

		public void Register(Type serviceType, ObjectDef def) {
			_register(serviceType, def);
		}

		public void Inject(Type abstraction, Type concretion) {
			RegisterType(abstraction, concretion);
		}


		public virtual IActionBehavior BuildBehavior(ServiceArguments arguments, Guid behaviorId) {
			return _context.Resolve<NestedAutofacContainerBehavior>(new TypedParameter(typeof(ServiceArguments), arguments),
			                                                        new TypedParameter(typeof(Guid), behaviorId));
		}

		public T Get<T>() {
			return _context.Resolve<T>();
		}

		public IEnumerable<T> GetAll<T>() {
			return _context.Resolve<IEnumerable<T>>();
		}


		private void RegisterInstance(Type type, object instance) {
			var builder = GetContainerBuilder();
			builder.Register(context => instance).As(type).InstancePerLifetimeScope();
			builder.Update(_context.ComponentRegistry);
		}

		private void RegisterType(Type abstraction, Type concretion) {
			var builder = GetContainerBuilder();
			builder.RegisterType(concretion).As(abstraction).InstancePerLifetimeScope();
			builder.Update(_context.ComponentRegistry);
		}


		private static ContainerBuilder GetContainerBuilder() {
			var builder = new ContainerBuilder();
			builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
			return builder;
		}
	}
}