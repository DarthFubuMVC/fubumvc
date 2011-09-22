using System;
using System.Linq.Expressions;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;
using FubuMVC.Core.Rest.Conneg;
using FubuMVC.Core.Rest.Media.Formatters;

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
            throw new NotImplementedException();
            //_fubuRegistry.Policies.Add(new ConnegBehaviorConvention(filter.Compile(), "BehaviorChain meets criteria:  " + filter.ToString()));
            //return this;
        }

        public MediaExpression ApplyContentNegotiationToActions(Expression<Func<ActionCall, bool>> filter)
        {
            throw new NotImplementedException();
            //var callPredicate = filter.Compile();
            //Func<BehaviorChain, bool> chainFilter = chain => chain.Calls.Any(callPredicate);
            //_fubuRegistry.Policies.Add(new ConnegBehaviorConvention(chainFilter, "Any action meets " + filter.ToString()));
            //return this;
        }

        public MediaExpression Formatter<T>() where T : IFormatter
        {
            _fubuRegistry.Services(x => x.AddService<IFormatter, T>());
            return this;
        }
    }
}