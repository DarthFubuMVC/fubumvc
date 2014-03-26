using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.UI.Bootstrap.Tags;
using FubuMVC.Diagnostics.Runtime;


namespace FubuMVC.Diagnostics.Requests
{
    public class BehaviorChainTraceTag : OutlineTag
    {
        public BehaviorChainTraceTag(BehaviorChain chain, RequestLog log)
        {
            AddHeader("Behaviors");

            chain.NonDiagnosticNodes().Each(node => Append(new BehaviorNodeTraceTag(node, log)));
        }


    }
}