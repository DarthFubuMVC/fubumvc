using System;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;

namespace FubuMVC.Core
{
    /// <summary>
    /// Explicitly marks this endpoint as "symmetric Json," meaning that it
    /// will only accept Json and output to Json
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SymmetricJsonAttribute : ModifyChainAttribute
    {
        public override void Alter(ActionCall call)
        {
            call.ParentChain().MakeSymmetricJson();
        }
    }
}