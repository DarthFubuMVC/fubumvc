using System;
using FubuCore;
using FubuTransportation.Configuration;
using FubuTransportation.Runtime;
using FubuTransportation.Runtime.Headers;
using FubuTransportation.Runtime.Serializers;

namespace FubuTransportation.Testing
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