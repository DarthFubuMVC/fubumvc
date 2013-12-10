using System;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;

namespace FubuMVC.Core
{
    /// <summary>
    /// Explicitly directs FubuMVC that this endpoint should support output to Json
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class JsonEndpointAttribute : ModifyChainAttribute
    {
        public override void Alter(ActionCall call)
        {
            call.ParentChain().OutputJson();
        }
    }
}