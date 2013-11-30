using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Windsor
{
    public class WindsorDependencyVisitor : IDependencyVisitor
    {
        private readonly ObjectDef _objectDef;
        private readonly ComponentRegistration _registration;
        private readonly IList<IRegistration> _components
            = new List<IRegistration>();
        
        public WindsorDependencyVisitor(Type serviceType, ObjectDef objectDef, bool isDependency)
        {
            _objectDef = objectDef;
            _registration = map(serviceType, objectDef, false);

            // NOTE[MT]: if class is registered as dependency
            // it should be registered as default service to pass 
            // auto_wiring_applies_even_when_another_dependency_is_set_explicitly test
            if (isDependency && serviceType.IsClass) 
                _components.Add(map(serviceType, objectDef, true));

            _registration = map(serviceType, objectDef, false);
            _components.Add(_registration);
        }

        public IRegistration[] Registrations()
        {
            _objectDef.AcceptVisitor(this);
            return _components.ToArray();
        }

        void IDependencyVisitor.Value(ValueDependency dependency)
        {
            _registration.DependsOn(Dependency.OnValue(dependency.DependencyType, dependency.Value));
        }

        void IDependencyVisitor.Configured(ConfiguredDependency dependency)
        {
            if (dependency.Definition.Value != null) {
                _registration.DependsOn(Dependency.OnValue(dependency.DependencyType, dependency.Definition.Value));
            }
            else {
                var v = new WindsorDependencyVisitor(dependency.DependencyType, dependency.Definition,true);
                _components.AddRange(v.Registrations());
                _registration.DependsOn(Dependency.OnComponent(dependency.DependencyType, dependency.Definition.Name));

            }
        }

        void IDependencyVisitor.List(ListDependency dependency)
        {
            _components.AddRange(dependency
                                    .Items
                                    .Select(def => new WindsorDependencyVisitor(dependency.ElementType, def, true))
                                    .SelectMany(v => v.Registrations()));

            _registration.DependsOn(Dependency.OnComponentCollection(dependency.DependencyType,
                                                                     dependency.Items.Select(def => def.Name).ToArray()));
        }

        private static ComponentRegistration map(Type serviceType, ObjectDef def, bool isDefault)
        {
            var c = Component.For(serviceType);

            if (def.Value != null) c.Instance(def.Value);
            else c.ImplementedBy(def.Type);

            if (isDefault) {
                c.IsDefault().LifestyleScoped();
            }
            else {
                c.Named(def.Name).IsFallback();
                if (def.IsSingleton) c.LifestyleSingleton();
                else c.LifestyleScoped();
            }
            return c;
        }

    }
}