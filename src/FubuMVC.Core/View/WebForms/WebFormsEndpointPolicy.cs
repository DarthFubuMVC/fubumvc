using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Util;

namespace FubuMVC.Core.View.WebForms
{
    // TODO:  Test for this turkey
    // Find any action w/o an output, look for the WebFormEndpoint att
    public class WebFormsEndpointPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Actions()
                .Where(x => !x.HasOutputBehavior())
                .Each(x => x.Method.ForAttribute<WebFormsEndpointAttribute>(att =>
                {
                    var token = new WebFormViewToken(att.ViewType);
                    x.Append(token.ToBehavioralNode());
                    graph.Observer.RecordCallStatus(x, 
                      "Action '{0}' has {1} declared, using WebForms view '{2}'".ToFormat(
                        x.Description, typeof(WebFormsEndpointAttribute).Name, token));
                }));
            }
    }
}