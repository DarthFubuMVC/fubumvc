using System;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration.Nodes
{
    public class ConditionalNode : BehaviorNode
    {
        private readonly Func<bool> _condition;
        private readonly BehaviorNode _node;
        private readonly Type _conditionalBehavior;
        private ObjectDef _objDef;

        public ConditionalNode(BehaviorNode node, Type conditionalBehavior)
        {
            _node = node;
            _conditionalBehavior = conditionalBehavior;
            if(!conditionalBehavior.CanBeCastTo<IConditionalBehavior>()) 
                throw new FubuAssertionException("Type must be of type ConditionalBehavior");

            _objDef = ObjectDef.ForType<ConditionalBehaviorInvoker>();
        }

        public ConditionalNode(BehaviorNode node, Func<bool> condition)
            : this(node, typeof(ConditionalBehavior))
        {
            _condition = condition;
         
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Conditional; }
        }

        protected override ObjectDef buildObjectDef()
        {
            var conditionalInvoker = new ObjectDef(_conditionalBehavior);
            conditionalInvoker.DependencyByType<IActionBehavior>(_node.As<IContainerModel>().ToObjectDef());
            if (_condition != null) conditionalInvoker.DependencyByValue(_condition);
            _objDef.DependencyByType<IConditionalBehavior>(conditionalInvoker);
            return _objDef;
        }
    }
  
    public class ConditionalNode<T> : BehaviorNode
    {
        private readonly BehaviorNode _node;
        private readonly Func<T, bool> _condition;
        private ObjectDef _objDef;


        public ConditionalNode(BehaviorNode node, Func<T, bool> condition)
        {
            _node = node;
            _condition = condition;
            _objDef = ObjectDef.ForType<ConditionalBehaviorInvoker>();
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Conditional; }
        }

        protected override ObjectDef buildObjectDef()
        {
            var conditionalInvoker = ObjectDef.ForType<ConditionalBehavior<T>>();
            conditionalInvoker.DependencyByType<IActionBehavior>(_node.As<IContainerModel>().ToObjectDef());
            conditionalInvoker.DependencyByValue(_condition);
            _objDef.DependencyByType<IConditionalBehavior>(conditionalInvoker);
            return _objDef;

        }
    }

   
}