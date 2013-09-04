using System;
using System.Collections.Generic;

using Autofac;
using Autofac.Features.ResolveAnything;

using FubuCore;
using FubuCore.Binding;

using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;


namespace FubuMVC.Autofac
{
    public class AutofacContainerFacility : IContainerFacility, IServiceFactory
    {
        private readonly IComponentContext _context;
        private readonly AutofacFubuModule _module;

        private Action<Type, ObjectDef> _register;


        public AutofacContainerFacility(IComponentContext context)
        {
            _context = context;
            _module = new AutofacFubuModule();

            _register = (serviceType, def) =>
            {
                bool isSingleton = (ServiceRegistry.ShouldBeSingleton(serviceType) || ServiceRegistry.ShouldBeSingleton(def.Type) || def.IsSingleton);
                _module.AddRegistration(serviceType, def, isSingleton);
            };

            _register(typeof (IServiceFactory), ObjectDef.ForValue(this));
        }


        public IComponentContext Context
        {
            get { return _context; }
        }


        public IServiceFactory BuildFactory()
        {
            var builder = GetContainerBuilder();
            builder.RegisterModule(_module);
            builder.Update(_context.ComponentRegistry);

            _register = UpdateRegistry;

            return this;
        }

        public void Register(Type serviceType, ObjectDef def)
        {
            _register(serviceType, def);
        }

        public void Shutdown()
        {
            
        }


        public virtual IActionBehavior BuildBehavior(ServiceArguments arguments, Guid behaviorId)
        {
            return _context.Resolve<NestedAutofacContainerBehavior>(
                new TypedParameter(typeof(ServiceArguments), arguments),
                new TypedParameter(typeof(Guid), behaviorId));
        }

        public T Get<T>()
        {
            return _context.Resolve<T>();
        }

        public IEnumerable<T> GetAll<T>()
        {
            return _context.Resolve<IEnumerable<T>>();
        }

        public void Dispose()
        {
        }


        private void RegisterType(Type abstraction, Type concretion)
        {
            var builder = GetContainerBuilder();

            if (concretion.IsOpenGeneric())
            {
                builder.RegisterGeneric(concretion).As(abstraction).InstancePerLifetimeScope();
            }
            else
            {
                builder.RegisterType(concretion).PreserveExistingDefaults().As(abstraction).InstancePerLifetimeScope();
            }

            builder.Update(_context.ComponentRegistry);
        }

        private void UpdateRegistry(Type abstraction, ObjectDef definition)
        {
            var builder = GetContainerBuilder();

            var registration = new ObjectDefRegistration(builder, definition, false, false);
            registration.Register(abstraction);

            builder.Update(_context.ComponentRegistry);
        }


        private static ContainerBuilder GetContainerBuilder()
        {
            var builder = new ContainerBuilder();
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            return builder;
        }
    }
}