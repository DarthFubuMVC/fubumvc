using System;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Core.Registration.Nodes
{
    public class Process : Wrapper
    {
        public new static Process For<T>() where T : IActionBehavior
        {
            return new Process(typeof(T));
        }

        public Process(Type behaviorType) : base(behaviorType)
        {
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Process; }
        }
    }
}