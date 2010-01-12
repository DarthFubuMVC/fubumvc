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
            graph.Actions().Where(x => !x.HasOutputBehavior()).Each(x =>
            {
                x.Method.ForAttribute<WebFormsEndpointAttribute>(att =>
                {
                    var token = new WebFormViewToken(att.ViewType);
                    x.Append(token.ToBehavioralNode());
                });
            });
        }
    }
}