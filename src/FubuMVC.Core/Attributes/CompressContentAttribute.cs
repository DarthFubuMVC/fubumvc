using System;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CompressContentAttribute : ModifyChainAttribute
    {
        public override void Alter(ActionCallBase call)
        {
            call.Chain.ApplyCompression();
        }
    }
}