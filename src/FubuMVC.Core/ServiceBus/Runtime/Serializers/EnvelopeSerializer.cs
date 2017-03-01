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
        object Deserialize(Envelope envelope, ChannelNode node);
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

        public object Deserialize(Envelope envelope, ChannelNode node)
        {
            if (envelope.Data == null) throw new EnvelopeDeserializationException("No data on this envelope to deserialize");

            var serializer = SelectSerializer(envelope, node);

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

        private IMessageSerializer findSerializer(string contentType, bool throws = true)
        {
            var serializer = _serializers.FirstOrDefault(x => x.ContentType.EqualsIgnoreCase(contentType));

            if (throws && serializer == null)
            {
                throw new EnvelopeDeserializationException("Unknown content-type '{0}'".ToFormat(contentType));
            }

            return serializer;
        }

        private IMessageSerializer findSerializerForContentTypes(IEnumerable<string> acceptedContentTypes, Envelope envelope, bool throws = true)
        {
            var validSerializers = acceptedContentTypes
                   .Select(type => findSerializer(type, false))
                   .Where(x => x != null)
                   .ToList();

            if (throws && !validSerializers.Any())
                throw new EnvelopeDeserializationException(
                    "No acceptable serializers registered for message {0} with the accepted content types {1}."
                    .ToFormat(envelope.Message.GetType().Name, string.Join(",", acceptedContentTypes)));

            //TODO: This could be smarter.
            return validSerializers.FirstOrDefault();
        }

        //TODO: Consider memoizing this.
        public IMessageSerializer SelectSerializer(Envelope envelope, ChannelNode node)
        {
            //Envelope
            if (envelope.ContentType.IsNotEmpty())
            {
                return findSerializer(envelope.ContentType);
            }
            if (envelope.AcceptedContentTypes.Any())
            {
                var serializer = findSerializerForContentTypes(envelope.AcceptedContentTypes, envelope, false);
                if(serializer != null) return serializer;
            }

            //Channel
            if (node.DefaultSerializer != null)
            {
                return node.DefaultSerializer;
            }
            if (node.DefaultContentType.IsNotEmpty())
            {
                return findSerializer(node.DefaultContentType);
            }
             if (node.AcceptedContentTypes != null && node.AcceptedContentTypes.Any())
            {
                var serializer = findSerializerForContentTypes(node.AcceptedContentTypes, envelope, false);
                if(serializer != null) return serializer;
            }

            //Graph
            if ( _graph.AcceptedContentTypes.Any())
            {
                return findSerializerForContentTypes(_graph.AcceptedContentTypes, envelope);
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
