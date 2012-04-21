using System;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.WebForms
{
    [AttributeUsage(AttributeTargets.Method)]
    public class WebFormsEndpointAttribute : ModifyChainAttribute
    {
        private readonly Type _viewType;

        public WebFormsEndpointAttribute(Type viewType)
        {
            _viewType = viewType;
        }

        public Type ViewType { get { return _viewType; } }
        public override void Alter(ActionCall call)
        {
            var webFormToken = new WebFormViewToken(ViewType);
            call.ParentChain().Output.AddView(webFormToken);
        }
    }
}