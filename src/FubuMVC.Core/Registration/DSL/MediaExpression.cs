using System;
using System.Linq;
using System.Linq.Expressions;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime.Formatters;
using System.Collections.Generic;
using FubuMVC.Core.Resources.Conneg;

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
            var policy = new MediaAttachmentPolicy(filter.Compile());
            _fubuRegistry.Policies.Add(policy);

            return this;
        }

        public MediaExpression ApplyContentNegotiationToActions(Expression<Func<ActionCall, bool>> filter)
        {
            var func = filter.Compile();
            var policy = new MediaAttachmentPolicy(chain => chain.Calls.Any(func));
            _fubuRegistry.Policies.Add(policy);

            return this;
        }

        public MediaExpression Formatter<T>() where T : IFormatter
        {
            _fubuRegistry.Services(x => x.AddService<IFormatter, T>());
            return this;
        }
    }

    // TODO -- let's get some diagnostics on this puppy
    public class MediaAttachmentPolicy : IConfigurationAction
    {
        private readonly Func<BehaviorChain, bool> _filter;

        public MediaAttachmentPolicy(Func<BehaviorChain, bool> filter)
        {
            _filter = filter;
        }

        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Where(_filter).Each(x => x.ApplyConneg());
        }
    }
}