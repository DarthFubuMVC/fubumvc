using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;
using FubuMVC.Core.ServiceBus.Subscriptions;
using HtmlTags;

namespace FubuMVC.Core.ServiceBus.Diagnostics.Visualization
{
    public class ChannelGraphFubuDiagnostics
    {
        private readonly ChannelGraph _graph;
        private readonly ISubscriptionCache _cache;
        private readonly IEnumerable<ITransport> _transports;
        private readonly IEnumerable<IMessageSerializer> _serializers;

        public ChannelGraphFubuDiagnostics(ChannelGraph graph, ISubscriptionCache cache, IEnumerable<ITransport> transports, IEnumerable<IMessageSerializer> serializers)
        {
            _graph = graph;
            _cache = cache;
            _transports = transports;
            _serializers = serializers;
        }

        [System.ComponentModel.Description("Transports and Channels:Visualizes the active model of all the channels and transports in this application")]
        public HtmlTag get_channels()
        {
            var div = new HtmlTag("div");

            var header = "Channels and Transports for Node \"{0}\", Id \"{1}\"".ToFormat(_graph.Name, _graph.NodeId);

            div.Add("h1").Text(header);
            div.Add("br");

            div.Add("h3").Text("Transports");
            div.Append(new TransportsTag(_transports));

            div.Add("h3").Text("Serializers");
            div.Append(new SerializersTag(_serializers));

            div.Add("h3").Text("Channels");
            div.Append(new ChannelsTableTag(_graph));

            div.Add("h3").Text("Subscriptions");
            div.Append(new SubscriptionsTableTag(_cache.ActiveSubscriptions));

            return div;
        }
    }
}