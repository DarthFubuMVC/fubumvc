using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.ServiceBus.Configuration;

namespace FubuMVC.Core.ServiceBus.Runtime
{
    public abstract class TransportBase
    {
        public abstract string Protocol { get; }

        public void OpenChannels(ChannelGraph graph)
        {
            var nodes = graph.NodesForProtocol(Protocol).ToArray();

            if (Disabled(nodes)) return;

            nodes.OrderByDescending(x => x.Incoming).Each(x => x.Channel = buildChannel(x));
            seedQueues(nodes);

            graph.AddReplyChannel(Protocol, getReplyUri(graph));
        }

        public virtual bool Disabled(IEnumerable<ChannelNode> nodes)
        {
            return false;
        }

        protected abstract Uri getReplyUri(ChannelGraph graph);

        protected abstract IChannel buildChannel(ChannelNode channelNode);

        protected abstract void seedQueues(IEnumerable<ChannelNode> channels);
    }
}