using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration.Nodes
{
    public class ConditionalNode : BehaviorNode
    {
        private ObjectDef _objDef;


        public ConditionalNode(Func<bool> condition)
        {
            _objDef = new ObjectDef(typeof(ConditionalBehavior));
            _objDef.DependencyByValue(condition);
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Conditional; }
        }

        protected override ObjectDef buildObjectDef()
        {
            return _objDef;
        }
    }
    public class ConditionalNode<T> : BehaviorNode
    {
        private ObjectDef _objDef;


        public ConditionalNode(Func<T,bool> condition)
        {
            _objDef = new ObjectDef(typeof(ConditionalBehavior<T>));
            _objDef.DependencyByValue(condition);
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Conditional; }
        }

        protected override ObjectDef buildObjectDef()
        {
            return _objDef;
        }
    }

   
}