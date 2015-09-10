using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Registration.Nodes;

namespace FubuMVC.Core.ServiceBus.Diagnostics.Visualization
{
    public class MessagesFubuDiagnostics
    {
        private readonly BehaviorGraph _graph;

        public MessagesFubuDiagnostics(BehaviorGraph graph, ChannelGraph channels)
        {
            _graph = graph;
        }

        [Description("Message Handlers:A representation of all the message types and handlers for this FubuTransportation node")]
        public Dictionary<string, object>[] get_messages()
        {
            return _graph.Handlers.Where(x => !x.IsPollingJob()).OrderBy(x => x.InputType().Name).Select(chain =>
            {
                var calls = chain.OfType<HandlerCall>().Select(x => x.Description).Join(", ");

                return new Dictionary<string, object>
                {
                    {"message_type", chain.InputType().Name},
                    {"full_name", chain.InputType().FullName},
                    {"hash", chain.Key},
                    {"handlers", calls}
                };
            }).ToArray();


        }
    }
}