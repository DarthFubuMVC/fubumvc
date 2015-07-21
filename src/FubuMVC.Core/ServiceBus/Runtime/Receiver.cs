using System;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime.Headers;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;

namespace FubuMVC.Core.ServiceBus.Runtime
{
    public class Receiver : IReceiver
    {
        private readonly IHandlerPipeline _pipeline;
        private readonly ChannelGraph _graph;
        private readonly ChannelNode _node;
        private readonly Uri _address;

        public Receiver(IHandlerPipeline pipeline, ChannelGraph graph, ChannelNode node)
        {
            _pipeline = pipeline;
            _graph = graph;
            _node = node;
            _address = node.Channel.Address;
        }

        public void Receive(byte[] data, IHeaders headers, IMessageCallback callback)
        {
            if (data == null) throw new ArgumentNullException("data");
            if (headers == null) throw new ArgumentNullException("headers");
            if (callback == null) throw new ArgumentNullException("callback");

            var envelope = new Envelope(data, headers, callback)
            {
                ReceivedAt = _address
            };

            envelope.ContentType = envelope.ContentType ?? _node.DefaultContentType ?? _graph.DefaultContentType;
            
            _pipeline.Receive(envelope);
        }

        protected bool Equals(Receiver other)
        {
            return Equals(_pipeline, other._pipeline) && Equals(_graph, other._graph) && Equals(_node, other._node);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Receiver) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_pipeline != null ? _pipeline.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_graph != null ? _graph.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_node != null ? _node.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}