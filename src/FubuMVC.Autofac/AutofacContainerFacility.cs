using System;
using System.Collections.Generic;

using Autofac;

using FubuCore.Binding;

using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;


namespace FubuMVC.Autofac {
	public class AutofacContainerFacility : IContainerFacility, IServiceFactory {
		private readonly IContainer _container;


		public AutofacContainerFacility(IContainer container) {
			_container = container;
		}


		public IServiceFactory BuildFactory() {
			throw new NotImplementedException();
		}

		public void Register(Type serviceType, ObjectDef def) {
			throw new NotImplementedException();
		}

		public void Inject(Type abstraction, Type concretion) {
			throw new NotImplementedException();
		}


		public IActionBehavior BuildBehavior(ServiceArguments arguments, Guid behaviorId) {
			throw new NotImplementedException();
		}

		public T Get<T>() {
			throw new NotImplementedException();
		}

		public IEnumerable<T> GetAll<T>() {
			throw new NotImplementedException();
		}
	}
}