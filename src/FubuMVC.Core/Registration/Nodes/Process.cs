using System;

namespace FubuMVC.Core.Registration.Nodes
{
    public class Process : Wrapper
    {
        public Process(Type behaviorType) : base(behaviorType)
        {
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Process; }
        }
    }
}