using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Registration;

namespace FubuMVC.WebForms
{
    // Find any action w/o an output, look for the WebFormEndpoint att
    public class WebFormsEndpointPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Actions()
                .Where(x => !x.HasAnyOutputBehavior())
                .Each(x => x.Method.ForAttribute<WebFormsEndpointAttribute>(att =>
                {
                    var token = new WebFormViewToken(att.ViewType);
                    x.AddToEnd(token.ToBehavioralNode());
                    graph.Observer.RecordCallStatus(x, 
                      "Action '{0}' has {1} declared, using WebForms view '{2}'".ToFormat(
                        x.Description, typeof(WebFormsEndpointAttribute).Name, token));
                }));
            }
    }
}