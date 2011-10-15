using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuCore;

namespace FubuMVC.Core.Registration.Nodes
{
    public class Wrapper : BehaviorNode
    {
        private readonly ObjectDef _objectDef;

        public Wrapper(Type behaviorType)
        {
            if (!behaviorType.CanBeCastTo<IActionBehavior>())
            {
                throw new ArgumentOutOfRangeException("behaviorType", "Only types that can be cast to IActionBehavior may be used here");
            }

            _objectDef = new ObjectDef
            {
                Type = behaviorType
            };
        }

        public Type BehaviorType { get { return _objectDef.Type; } }

        public override BehaviorCategory Category { get { return BehaviorCategory.Wrapper; } }

        public static Wrapper For<T>() where T : IActionBehavior
        {
            return new Wrapper(typeof (T));
        }

        protected override ObjectDef buildObjectDef()
        {
            return _objectDef;
        }

        public override string ToString()
        {
            return "Wrapped by " + _objectDef.Type.FullName;
        }
    }
}