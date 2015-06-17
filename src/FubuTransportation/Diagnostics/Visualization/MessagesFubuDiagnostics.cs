using System.ComponentModel;
using FubuTransportation.Configuration;
using HtmlTags;

namespace FubuTransportation.Diagnostics.Visualization
{
    public class MessagesFubuDiagnostics
    {
        private readonly HandlerGraph _graph;

        public MessagesFubuDiagnostics(HandlerGraph graph)
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