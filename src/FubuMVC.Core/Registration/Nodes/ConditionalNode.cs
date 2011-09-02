using System;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration.Nodes
{
    public class ConditionalNode : BehaviorNode
    {
        private readonly BehaviorNode _innerNode;

        public ConditionalNode(BehaviorNode innerNode)
        {
            _innerNode = innerNode;
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Conditional; }
        }

        protected override ObjectDef buildObjectDef()
        {
            throw new NotImplementedException();
        }
    }
}