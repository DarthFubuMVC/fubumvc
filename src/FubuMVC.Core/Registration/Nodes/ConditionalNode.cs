using System;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration.Nodes
{
    public class ConditionalNode : BehaviorNode
    {
        private readonly BehaviorNode _node;
        private ObjectDef _objDef;

        public ConditionalNode(BehaviorNode node, Type conditionalBehavior)
        {
            _node = node;
            if(!conditionalBehavior.CanBeCastTo<ConditionalBehavior>()) 
                throw new FubuAssertionException("Type must be of type ConditionalBehavior");

            _objDef = new ObjectDef(conditionalBehavior);
        }

        public ConditionalNode(BehaviorNode node, Func<bool> condition)
            : this(node, typeof(ConditionalBehavior))
        {
            _objDef.DependencyByValue(condition);
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Conditional; }
        }

        protected override ObjectDef buildObjectDef()
        {
            _objDef.Dependency(typeof(IActionBehavior), _node.As<IContainerModel>().ToObjectDef());
            return _objDef;
        }
    }
  
    public class ConditionalNode<T> : BehaviorNode
    {
        private readonly BehaviorNode _node;
        private ObjectDef _objDef;


        public ConditionalNode(BehaviorNode node, Func<T, bool> condition)
        {
            _node = node;
            _objDef = new ObjectDef(typeof(ConditionalBehavior<T>));
            _objDef.DependencyByValue(condition);
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Conditional; }
        }

        protected override ObjectDef buildObjectDef()
        {
            _objDef.Dependency(typeof(IActionBehavior), _node.As<IContainerModel>().ToObjectDef());
            return _objDef;
        }
    }

   
}