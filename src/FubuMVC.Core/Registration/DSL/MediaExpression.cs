using System;
using System.Linq;
using System.Linq.Expressions;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Core.Registration.DSL
{
    public class MediaExpression
    {
        private readonly FubuRegistry _fubuRegistry;

        public MediaExpression(FubuRegistry fubuRegistry)
        {
            _fubuRegistry = fubuRegistry;
        }

        public MediaExpression ApplyContentNegotiationTo(Expression<Func<BehaviorChain, bool>> filter)
        {
            throw new NotImplementedException("NWO");
            //_policy.AddFilter("Behavior chain matches " + filter, filter.Compile());
            return this;
        }

        public MediaExpression ApplyContentNegotiationToActions(Expression<Func<ActionCall, bool>> filter)
        {
            throw new NotImplementedException("NWO");
            var func = filter.Compile();
            //_policy.AddFilter("Action matches " + filter, chain => chain.Calls.Any(func));
            
            return this;
        }

        public MediaExpression Formatter<T>() where T : IFormatter
        {
            _fubuRegistry.Services(x => x.AddService<IFormatter, T>());
            return this;
        }
    }
}