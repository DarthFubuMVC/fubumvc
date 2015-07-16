using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;

namespace FubuMVC.Core.Registration
{
    public class OverridesFor<T> : IAccessorRulesRegistration
    {
        private readonly IList<Action<AccessorRules>> _registrations = new List<Action<AccessorRules>>();

        public AccessorRulesExpression<T> Property(Expression<Func<T, object>> expression)
        {
            var accessor = expression.ToAccessor();
            Action<object> registration = o => _registrations.Add(r => r.Add(accessor, o));

            return new AccessorRulesExpression<T>(registration);
        }

        void IAccessorRulesRegistration.AddRules(AccessorRules rules)
        {
            _registrations.Each(x => x(rules));
        }
    }
}