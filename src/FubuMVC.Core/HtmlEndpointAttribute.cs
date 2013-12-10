using System;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core
{
    /// <summary>
    /// Explicitly marks this endpoint as returning Html by calling 
    /// the ToString() method on the output as mimetype "text/html"
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HtmlEndpointAttribute : ModifyChainAttribute
    {
        public override void Alter(ActionCall call)
        {
            var html = call.ParentChain().Output.AddHtml();
            html.MoveToFront();
        }
    }
}