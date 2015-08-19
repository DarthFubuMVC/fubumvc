using System.ComponentModel;
using FubuMVC.Core.Registration;
using FubuMVC.Core.ServiceBus.Configuration;
using HtmlTags;

namespace FubuMVC.Core.ServiceBus.Diagnostics.Visualization
{
    public class MessagesFubuDiagnostics
    {
        private readonly BehaviorGraph _graph;

        public MessagesFubuDiagnostics(BehaviorGraph graph)
        {
            _graph = graph;
        }

        [Description("Message Handlers:A representation of all the message types and handlers for this FubuTransportation node")]
        public HtmlTag get_messages()
        {
            return new HtmlTag("div", div => {
                div.Add("h1").Text("Message Handler Chains");
                div.Append(new HandlersTableTag(_graph));
            });
        }
    }
}