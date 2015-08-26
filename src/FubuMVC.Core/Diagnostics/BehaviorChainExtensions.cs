using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Diagnostics
{
    public static class BehaviorChainExtensions
    {
        public static IEnumerable<BehaviorNode> NonDiagnosticNodes(this BehaviorChain chain)
        {
            return chain.Where(NonDiagnosticNodes);
        }

        public static bool NonDiagnosticNodes(BehaviorNode node)
        {
            if (node is BehaviorTracerNode) return false;

            return true;
        }
    }
}