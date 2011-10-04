using System;
using System.Linq;
using System.Linq.Expressions;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Media.Formatters;

namespace FubuMVC.Core.Registration.DSL
{
    public class MediaExpression
    {
        private readonly FubuRegistry _fubuRegistry;
        private readonly ConnegAttachmentPolicy _policy;

        public MediaExpression(FubuRegistry fubuRegistry, ConnegAttachmentPolicy policy)
        {
            _fubuRegistry = fubuRegistry;
            _policy = policy;
        }

        public MediaExpression ApplyContentNegotiationTo(Expression<Func<BehaviorChain, bool>> filter)
        {
            _policy.AddFilter("Behavior chain matches " + filter, filter.Compile());
            return this;
        }

        public MediaExpression ApplyContentNegotiationToActions(Expression<Func<ActionCall, bool>> filter)
        {
            var func = filter.Compile();
            _policy.AddFilter("Action matches " + filter, chain => chain.Calls.Any(func));
            
            return this;
        }

        public MediaExpression Formatter<T>() where T : IFormatter
        {
            _fubuRegistry.Services(x => x.AddService<IFormatter, T>());
            return this;
        }
    }
}