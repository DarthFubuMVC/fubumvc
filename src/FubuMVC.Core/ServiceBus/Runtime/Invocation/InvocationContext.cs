using System;
using System.Collections.Generic;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServiceBus.Configuration;
using StructureMap.Pipeline;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public class InvocationContext : TypeArguments, IInvocationContext
    {
        private static readonly IDictionary<string, object> _emptyDictionary = new Dictionary<string, object>();

        private readonly IList<object> _messages = new List<object>();

        public InvocationContext(Envelope envelope, HandlerChain chain)
        {
            if (envelope == null) throw new ArgumentNullException(nameof(envelope));

            if (envelope.Log != null)
            {
                Set(typeof(IChainExecutionLog), envelope.Log);
            }

            var currentChain = new CurrentChain(chain, _emptyDictionary);
            Set(typeof(ICurrentChain), currentChain);
            
            Envelope = envelope;
            var inputType = envelope.Message.GetType();
            var request = new InMemoryFubuRequest();
            request.Set(inputType, Envelope.Message);
            
            Set(typeof(IFubuRequest), request);
            Set(typeof(IInvocationContext), this);
            Set(typeof(Envelope), envelope);
        }

        public Envelope Envelope { get; }

        public IContinuation Continuation { get; set; }

        public void EnqueueCascading(object message)
        {
            if (message == null) return;

            var enumerable = message as IEnumerable<object>;
            if (enumerable == null)
            {
                _messages.Add(message);
            }
            else
            {
                _messages.AddRange(enumerable);
            }
        }

        public IEnumerable<object> OutgoingMessages()
        {
            return _messages;
        }

        protected bool Equals(InvocationContext other)
        {
            return Equals(Envelope, other.Envelope);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((InvocationContext) obj);
        }

        public override int GetHashCode()
        {
            return (Envelope != null ? Envelope.GetHashCode() : 0);
        }
    }
}