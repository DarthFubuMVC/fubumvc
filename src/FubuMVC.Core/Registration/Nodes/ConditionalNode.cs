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

        public Func<bool> Condition
        {
            get { return _condition; }
        }

        public BehaviorNode InnerNode
        {
            get { return _node; }
        }

        public Type BehaviorType
        {
            get { return _conditionalBehavior; }
        }

        protected override ObjectDef buildObjectDef()
        {
            var conditionalInvoker = new ObjectDef(BehaviorType);
            conditionalInvoker.DependencyByType<IActionBehavior>(InnerNode.As<IContainerModel>().ToObjectDef());
            if (Condition != null)
            {
                var conditionalDef = ObjectDef.ForType<LambdaConditional>();
                conditionalDef.DependencyByValue(Condition);
                conditionalInvoker.DependencyByType<IConditional>(conditionalDef);
            }
            _objDef.DependencyByType<IConditionalBehavior>(conditionalInvoker);
            return _objDef;
        }
    }
  
    public class ConditionalNode<T> : BehaviorNode
    {
        private readonly BehaviorNode _node;
        private readonly Type _conditional;
        private readonly Func<T, bool> _condition;
        private ObjectDef _objDef;

        public ConditionalNode(BehaviorNode node, Type conditional)
        {
            _node = node;
            _conditional = conditional; 
            _objDef = ObjectDef.ForType<ConditionalBehaviorInvoker>();
        }

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

        public BehaviorNode InnerNode
        {
            get { return _node; }
        }

        public Func<T, bool> Condition
        {
            get { return _condition; }
        }

        protected override ObjectDef buildObjectDef()
        {
            var conditionalInvoker = ObjectDef.ForType<ConditionalBehavior>();
            conditionalInvoker.DependencyByType<IActionBehavior>(InnerNode.As<IContainerModel>().ToObjectDef());
            if (Condition != null)
            {
                var conditionalDef = ObjectDef.ForType<LambdaConditional<T>>();
                conditionalDef.DependencyByValue(Condition);
                conditionalInvoker.DependencyByType<IConditional>(conditionalDef);
            }
            else
            {
                conditionalInvoker.DependencyByType<IConditional>(new ObjectDef(_conditional));
            }
            _objDef.DependencyByType<IConditionalBehavior>(conditionalInvoker);
            return _objDef;

        }
    }

   
}