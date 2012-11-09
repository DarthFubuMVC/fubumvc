using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Bottles;
using FubuCore.Reflection;
using FubuCore;

namespace FubuMVC.Core.Registration
{
    [ConfigurationType(ConfigurationType.Discovery)]
    public class AccessorOverridesFinder : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var rules = graph.Settings.Get<AccessorRules>();

            var types = new TypePool();
            types.AddAssembly(graph.ApplicationAssembly);
            types.AddAssemblies(PackageRegistry.PackageAssemblies);

            types.TypesMatching(x => x.CanBeCastTo<IAccessorRulesRegistration>() && x.IsConcreteWithDefaultCtor()).
                Distinct().Select(x => {
                    return Activator.CreateInstance(x).As<IAccessorRulesRegistration>();
                })
                .Each(x => x.AddRules(rules));

            graph.Services.AddService(rules);
        }
    }

    public interface IAccessorRulesRegistration
    {
        void AddRules(AccessorRules rules);
    }

    public class OverridesFor<T> : IAccessorRulesRegistration
    {
        private readonly IList<Action<AccessorRules>> _registrations = new List<Action<AccessorRules>>();

        public AccessorRulesExpression Property(Expression<Func<T, object>> expression)
        {
            var accessor = expression.ToAccessor();
            Action<object> registration = o => _registrations.Add(r => r.Add(accessor, o));

            return new AccessorRulesExpression(registration);
        }

        public class AccessorRulesExpression
        {
            private readonly Action<object> _registration;

            public AccessorRulesExpression(Action<object> registration)
            {
                _registration = registration;
            }

            public AccessorRulesExpression Add(object rule)
            {
                _registration(rule);
                return this;
            }

            public AccessorRulesExpression Add<T>() where T : new()
            {
                return Add(new T());
            }
        }

        void IAccessorRulesRegistration.AddRules(AccessorRules rules)
        {
            _registrations.Each(x => x(rules));
        }
    }
}