using System;
using FubuCore.Descriptions;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuCore;

namespace FubuMVC.Core.Registration.Nodes
{
    public class Wrapper : BehaviorNode, DescribesItself
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

        void DescribesItself.Describe(Description description)
        {
            description.Title = BehaviorType.Name;
            description.ShortDescription = BehaviorType.FullName;
        }
    }
}