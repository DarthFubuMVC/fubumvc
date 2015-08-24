using System;
using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;

namespace FubuMVC.Tests.Registration.Conventions
{
    public static class BehaviorGraphExtensions
    {
        public static BehaviorChain AddChainForWriter<T>(this BehaviorGraph graph, object writer)
        {
            var chain = new BehaviorChain();
            var node = new OutputNode(typeof (T));
            node.Add((IMediaWriter) writer);

            chain.AddToEnd(node);

            graph.AddChain(chain);

            return chain;
        }

        public static void WrapAllWith<T>(this BehaviorGraph graph) where T : IActionBehavior
        {
            graph.Chains.Each(x => x.WrapWith<T>());
        }
    }
}