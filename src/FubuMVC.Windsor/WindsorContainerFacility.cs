using System;
using System.Collections.Generic;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Windsor
{
    public class WindsorContainerFacility : IContainerFacility, IServiceFactory
    {
        private readonly IWindsorContainer _windsorContainer;
        private Action<Type, ObjectDef> _register;
        private readonly WindsorFubuInstaller _installer;

        public WindsorContainerFacility(IWindsorContainer windsorContainer)
        {
            if (windsorContainer == null) throw new ArgumentNullException("windsorContainer");
            windsorContainer.Kernel.Resolver.AddSubResolver(new CollectionResolver(windsorContainer.Kernel));
            windsorContainer.Register(Component.For<ILazyComponentLoader>().ImplementedBy<WindsorLazyComponentLoader>());

            _windsorContainer = windsorContainer;

            _installer = new WindsorFubuInstaller();
            _windsorContainer.Register(Component.For<IServiceFactory>().Instance(this),
                                       Component.For<NestedWindsorContainerBehavior>().LifestyleTransient());

            _register = (type, def) => _installer.Add(type, def);

        }

        public IServiceFactory BuildFactory()
        {
            _windsorContainer.Install(_installer);
            _register = (type, def) => _windsorContainer.Register(type, def);
            return this;
        }

        public void Register(Type serviceType, ObjectDef def)
        {
            _register(serviceType, def);
        }

        public void Shutdown()
        {
            
        }

        public void Dispose()
        {
            _windsorContainer.Dispose();
        }

        public IActionBehavior BuildBehavior(ServiceArguments arguments, Guid behaviorId)
        {
            return _windsorContainer.Resolve<NestedWindsorContainerBehavior>(new { windsorContainer=_windsorContainer, arguments, behaviorId });
        }


        public T Get<T>()
        {
            _windsorContainer.RequireScope();
            return _windsorContainer.Resolve<T>();
        }

        public IEnumerable<T> GetAll<T>()
        {
            _windsorContainer.RequireScope();
            return _windsorContainer.ResolveAll<T>();
        }
    }
}