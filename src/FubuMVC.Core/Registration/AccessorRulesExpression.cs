using System;

namespace FubuMVC.Core.Registration
{
    public class AccessorRulesExpression<T> : IAccessorRulesExpression
    {
        private readonly Action<object> _registration;

        public AccessorRulesExpression(Action<object> registration)
        {
            _registration = registration;
        }

        public IAccessorRulesExpression Add(object rule)
        {
            _registration(rule);
            return this;
        }

        public IAccessorRulesExpression Add<TRule>() where TRule : new()
        {
            return Add(new TRule());
        }
    }
}