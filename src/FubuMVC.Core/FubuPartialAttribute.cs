using System;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core
{
    /// <summary>
    /// Explicitly marks an endpoint as "partial only"
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class FubuPartialAttribute : ModifyChainAttribute
    {
        public override void Alter(ActionCall call)
        {
            call.ParentChain().IsPartialOnly = true;
        }
    }
}