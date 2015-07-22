using System;
using FubuCore.Descriptions;
using FubuMVC.Core.Behaviors;
using FubuCore;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Registration.Nodes
{
    public class Wrapper : BehaviorNode, DescribesItself
    {
        private readonly ConstructorInstance _instance;

        public Wrapper(Type behaviorType)
        {
            if (!behaviorType.CanBeCastTo<IActionBehavior>())
            {
                throw new ArgumentOutOfRangeException("behaviorType", "Only types that can be cast to IActionBehavior may be used here");
            }

            _instance = new ConstructorInstance(behaviorType);
        }

        /// <summary>
        /// The type of the wrapping behavior appliec
        /// </summary>
        public Type Type
        {
            get
            {
                return _instance.ReturnedType;
            }
        }

        public override BehaviorCategory Category { get { return BehaviorCategory.Wrapper; } }

        public static Wrapper For<T>() where T : IActionBehavior
        {
            return new Wrapper(typeof (T));
        }


        protected override IConfiguredInstance buildInstance()
        {
            return _instance;
        }

        public override string ToString()
        {
            return "Wrapped by " + Type.FullName;
        }

        void DescribesItself.Describe(Description description)
        {
            description.Title = BehaviorType.Name;
            description.ShortDescription = BehaviorType.FullName;
        }
    }
}