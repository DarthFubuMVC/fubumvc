using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Logging;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;
using FubuMVC.Core.ServiceBus.Subscriptions;

namespace FubuMVC.Core.ServiceBus.Runtime
{
    public class EnvelopeSender : IEnvelopeSender
    {
        private readonly ISubscriptionCache _router;
        private readonly IEnvelopeSerializer _serializer;
        private readonly ILogger _logger;
        private readonly IEnumerable<IEnvelopeModifier> _modifiers;

        public EnvelopeSender(ISubscriptionCache router, IEnvelopeSerializer serializer, ILogger logger, IEnumerable<IEnvelopeModifier> modifiers)
        {
            _router = router;
            _serializer = serializer;
            _logger = logger;
            _modifiers = modifiers;
        }

        // virtual for testing
        public string Send(Envelope envelope)
        {
            envelope.Headers[Envelope.MessageTypeKey] = envelope.Message.GetType().FullName;

            _modifiers.Each(x => x.Modify(envelope));

            var channels = _router.FindDestinationChannels(envelope).ToArray();

            if (!channels.Any())
            {
                throw new Exception("No channels match this message ({0})".ToFormat(envelope));
            }

            channels.Each(x => {
                try
                {
                    sendToChannel(envelope, x);
                }
                catch (Exception e)
                {
                    _logger.Error(envelope.CorrelationId, "Failed trying to send message {0} to channel {1}".ToFormat(envelope, x.Uri), e);
                    throw;
                }
            });

            return envelope.CorrelationId;
        }

        /*
         * Changes
         * 1.) Do serialization within sendToChannel
         * 2.) do the cloning *outside* of sendToChannel
         * 3.) Make envelopeserializer smart enough not to replace the contents if it needs to
         */

        private void sendToChannel(Envelope envelope, ChannelNode node)
        {
            var replyUri = _router.ReplyUriFor(node);

            var headers = node.Send(envelope, _serializer, replyUri: replyUri);
            _logger.InfoMessage(() => new EnvelopeSent(new EnvelopeToken
            {
                Headers = headers,
                Message = envelope.Message
            }, node));
        }
    }
}