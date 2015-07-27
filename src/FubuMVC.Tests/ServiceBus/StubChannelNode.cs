using System;
using FubuCore;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Headers;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;

namespace FubuMVC.Tests.ServiceBus
{
    public class StubChannelNode : ChannelNode
    {
        public Envelope LastEnvelope;

        public StubChannelNode(string protocol = null)
        {
            Key = Guid.NewGuid().ToString();
            Uri = ("{0}://{1}".ToFormat(protocol ?? "fake", Key)).ToUri();
        }

        public override IHeaders Send(Envelope envelope, IEnvelopeSerializer serializer, Uri replyUri = null)
        {
            if (replyUri != null)
            {
                envelope.ReplyUri = replyUri;
            }

            LastEnvelope = envelope;

            return envelope.Headers;
        }
    }
}