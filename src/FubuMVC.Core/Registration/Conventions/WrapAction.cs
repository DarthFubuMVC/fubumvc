using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;
using System.Collections.Generic;

namespace FubuMVC.Core.Registration.Conventions
{
    [Policy]
    public class WrapAction<T> : IConfigurationAction where T : IActionBehavior
    {
        private readonly Func<BehaviorChain, bool> _filter;
        // TODO -- add more description here
        public WrapAction(Func<BehaviorChain, bool> filter)
        {
            _filter = filter;
        }

        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Where(_filter).Each(x => x.Prepend(Wrapper.For<T>()));
        }
    }
}