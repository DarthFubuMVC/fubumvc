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
            node.Add(writer);

            chain.AddToEnd(node);

            graph.AddChain(chain);

            return chain;
        }
    }
}