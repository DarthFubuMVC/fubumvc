using System;
using System.Linq.Expressions;
using FubuMVC.Core.Conneg;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;

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
            _fubuRegistry.Policies.Add(new ConnegBehaviorConvention(filter.Compile(), "BehaviorChain meets criteria:  " + filter.ToString()));
            return this;
        }

        public MediaExpression ApplyContentNegotiationToActions(Expression<Func<ActionCall, bool>> filter)
        {
            var callPredicate = filter.Compile();
            Func<BehaviorChain, bool> chainFilter = chain => chain.Calls.Any(callPredicate);
            _fubuRegistry.Policies.Add(new ConnegBehaviorConvention(chainFilter, "Any action meets " + filter.ToString()));
            return this;
        }

        public MediaExpression Formatter<T>() where T : IFormatter
        {
            _fubuRegistry.Services(x => x.AddService<IFormatter, T>());
            return this;
        }
    }
}