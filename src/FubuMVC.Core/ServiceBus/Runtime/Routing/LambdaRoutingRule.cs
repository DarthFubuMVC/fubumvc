using System;
using System.Linq.Expressions;

namespace FubuMVC.Core.ServiceBus.Runtime.Routing
{
    public class LambdaRoutingRule : IRoutingRule
    {
        private readonly Func<Type, bool> _filter;
        private readonly Expression<Func<Type, bool>> _expression;

        public LambdaRoutingRule(Expression<Func<Type, bool>> filter)
        {
            _filter = filter.Compile();
            _expression = filter;
        }

        public bool Matches(Type type)
        {
            return _filter(type);
        }

        public string Describe()
        {
            return "Messages matching " + _expression.ToString();
        }
    }
}