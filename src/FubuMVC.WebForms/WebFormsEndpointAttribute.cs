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
            var token = new WebFormViewToken(ViewType);
            call.ParentChain().AddToEnd(token.ToBehavioralNode());

            // TODO -- add some tracing back here.
            /*
             * 
                    graph.Observer.RecordCallStatus(x, 
                      "Action '{0}' has {1} declared, using WebForms view '{2}'".ToFormat(
                        x.Description, typeof(WebFormsEndpointAttribute).Name, token));
             * 
             */
        }
    }
}