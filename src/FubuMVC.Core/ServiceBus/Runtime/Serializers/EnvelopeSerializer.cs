using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Core.ServiceBus.Configuration;

namespace FubuMVC.Core.ServiceBus.Runtime.Serializers
{
    public interface IEnvelopeSerializer
    {
        object Deserialize(Envelope envelope);
        void Serialize(Envelope envelope, ChannelNode node);
    }

    public class EnvelopeSerializer : IEnvelopeSerializer
    {
        private readonly ChannelGraph _graph;
        private readonly IEnumerable<IMessageSerializer> _serializers;

        public EnvelopeSerializer(ChannelGraph graph, IEnumerable<IMessageSerializer> serializers)
        {
            _graph = graph;
            _serializers = serializers;
        }

        // TODO -- take in ChannelNode here!
        public object Deserialize(Envelope envelope)
        {
            if (envelope.Data == null) throw new EnvelopeDeserializationException("No data on this envelope to deserialize");


            var serializer = SelectSerializer(envelope, null);
            
            using (var stream = new MemoryStream(envelope.Data))
            {
                try
                {
                    return serializer.Deserialize(stream);
                }
                catch (Exception ex)
                {
                    throw new EnvelopeDeserializationException("Message serializer has failed", ex);
                }
            }
        }

        private IMessageSerializer findSerializer(string contentType)
        {
            var serializer = _serializers.FirstOrDefault(x => x.ContentType.EqualsIgnoreCase(contentType));


            if (serializer == null)
            {
                throw new EnvelopeDeserializationException("Unknown content-type '{0}'".ToFormat(contentType));
            }

            return serializer;
        }

        public IMessageSerializer SelectSerializer(Envelope envelope, ChannelNode node)
        {
            if (envelope.ContentType.IsNotEmpty())
            {
                return findSerializer(envelope.ContentType);
            }

            if (node.DefaultSerializer != null)
            {
                return node.DefaultSerializer;
            }

            if (node.DefaultContentType.IsNotEmpty())
            {
                return findSerializer(node.DefaultContentType);
            }

            return findSerializer(_graph.DefaultContentType);
        }

        public void Serialize(Envelope envelope, ChannelNode node)
        {
            if (envelope.Message == null) throw new InvalidOperationException("No message on this envelope to serialize");

            var serializer = SelectSerializer(envelope, node);
            if (envelope.ContentType.IsEmpty())
            {
                envelope.ContentType = serializer.ContentType;
            }

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(envelope.Message, stream);
                stream.Position = 0;

                envelope.Data = stream.ReadAllBytes();
            }
        }
    }
}