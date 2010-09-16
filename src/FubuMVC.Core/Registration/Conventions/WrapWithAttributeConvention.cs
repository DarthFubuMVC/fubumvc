using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;
using FubuCore.Reflection;

namespace FubuMVC.Core.Registration.Conventions
{
    public class WrapWithAttributeConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Each(scanChain);
        }

        private static void scanChain(BehaviorChain chain)
        {
            chain.Calls
                .SelectMany(x => x.Method.GetAllAttributes<WrapWithAttribute>())
                .SelectMany(x => x.BehaviorTypes.Select(t => new Wrapper(t)))
                .Reverse()
                .Each(chain.Prepend);
        }
    }
}