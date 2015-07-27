using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Delayed;
using LightningQueues.Model;

namespace FubuMVC.LightningQueues
{
    public class LightningQueuesTransport : TransportBase, ITransport
    {
        public static readonly string DelayedQueueName = "delayed";
        public static readonly string ErrorQueueName = "errors";

        private readonly IPersistentQueues _queues;
        private readonly LightningQueueSettings _settings;
        private readonly IDelayedMessageCache<MessageId> _delayedMessages;

        public LightningQueuesTransport(IPersistentQueues queues, LightningQueueSettings settings, IDelayedMessageCache<MessageId> delayedMessages)
        {
            _queues = queues;
            _settings = settings;
            _delayedMessages = delayedMessages;
        }

        public void Dispose()
        {
            // IPersistentQueues is disposable
        }

        public override string Protocol
        {
            get { return LightningUri.Protocol; }
        }

        public override bool Disabled(IEnumerable<ChannelNode> nodes)
        {
            if (_settings.Disabled) return true;

            if (!nodes.Any() && _settings.DisableIfNoChannels) return true;

            return false;
        }

        public IChannel BuildDestinationChannel(Uri destination)
        {
            return new LightningQueuesReplyChannel(destination, _queues.ManagerForReply());
        }

        public IEnumerable<EnvelopeToken> ReplayDelayed(DateTime currentTime)
        {
            return _queues.ReplayDelayed(currentTime);
        }

        public void ClearAll()
        {
            _queues.ClearAll();
        }

        protected override IChannel buildChannel(ChannelNode channelNode)
        {
            return LightningQueuesChannel.Build(new LightningUri(channelNode.Uri), _queues, _delayedMessages, channelNode.Incoming);
        }

        protected override void seedQueues(IEnumerable<ChannelNode> channels)
        {
            _queues.Start(channels.Where(x => x.Incoming).Select(x => new LightningUri(x.Uri)));
        }

        protected override Uri getReplyUri(ChannelGraph graph)
        {
            var channelNode = graph.FirstOrDefault(x => x.Protocol() == LightningUri.Protocol && x.Incoming);
            if (channelNode == null)
                throw new InvalidOperationException("You must have at least one incoming Lightning Queue channel for accepting replies");

            return channelNode.Channel.Address.ToLocalUri();
        }
    }
}