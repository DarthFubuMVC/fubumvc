using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core
{
    /// <summary>
    /// Applies one or more "Wrapper" behaviors of the given types to 
    /// this chain.  The Wrapper nodes are applied immediately before
    /// this ActionCall, from inside to outside
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class WrapWithAttribute : ModifyChainAttribute
    {
        private readonly Type[] _behaviorTypes;

        public WrapWithAttribute(params Type[] behaviorTypes)
        {
            _behaviorTypes = behaviorTypes;
        }

        public Type[] BehaviorTypes
        {
            get { return _behaviorTypes; }
        }

        public override void Alter(ActionCall call)
        {
            _behaviorTypes.Each(x => call.WrapWith(x));
        }
    }
}