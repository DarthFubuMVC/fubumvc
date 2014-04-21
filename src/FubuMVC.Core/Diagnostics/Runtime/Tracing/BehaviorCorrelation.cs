using System;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Diagnostics.Runtime.Tracing
{
    public class BehaviorCorrelation
    {
        public BehaviorCorrelation(BehaviorNode node)
        {
            var chain = node.ParentChain();
            ChainId = chain == null ? Guid.Empty : chain.UniqueId;

            Node = node;
        }

        public Guid ChainId { get; set; }
        public BehaviorNode Node { get; private set; }
    }
}