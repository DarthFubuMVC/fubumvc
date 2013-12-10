using System;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;

namespace FubuMVC.Core
{
    /// <summary>
    /// Explicitly marks this endpoint as asymmetric Json, meaning that it
    /// accepts either form posts or Json posts and outputs json
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AsymmetricJsonAttribute : ModifyChainAttribute
    {
        public override void Alter(ActionCall call)
        {
            call.ParentChain().MakeAsymmetricJson();
        }
    }
}