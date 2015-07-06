using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Runtime.Aggregation
{
    public class ClientRequest
    {
        public string type { get; set; }
        public object query { get; set; }
    }

    public class ClientMessagePath
    {
        public string Message { get; set; }
        public Type InputType { get; set; }
        public Type ResourceType { get; set; }

        protected bool Equals(ClientMessagePath other)
        {
            return string.Equals(Message, other.Message) && Equals(InputType, other.InputType) && Equals(ResourceType, other.ResourceType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ClientMessagePath) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Message != null ? Message.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (InputType != null ? InputType.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ResourceType != null ? ResourceType.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("Message: {0}, InputType: {1}, ResourceType: {2}", Message, InputType, ResourceType);
        }
    }

    public interface IClientMessageCache
    {
        BehaviorChain FindChain(string messageName);
        IEnumerable<ClientMessagePath> AllClientMessages();
    }

    // Tested through integration testing
    public class ClientMessageCache : IClientMessageCache
    {
        private readonly Cache<string, BehaviorChain> _chains = new Cache<string, BehaviorChain>();

        public ClientMessageCache(BehaviorGraph graph)
        {
            graph.Behaviors.Where(x => x.IsAggregatedChain()).Each(x =>
            {
                if (x.InputType().IsClientMessage())
                {
                    _chains[x.InputType().GetMessageName()] = x;
                }
                else
                {
                    _chains[x.ResourceType().GetMessageName()] = x;
                }
            });
        }

        public BehaviorChain FindChain(string messageName)
        {
            return _chains[messageName];
        }

        public IEnumerable<ClientMessagePath> AllClientMessages()
        {
            var list = new List<ClientMessagePath>();
            _chains.Each((name, chain) =>
            {
                list.Add(new ClientMessagePath
                {
                    Message = name,
                    InputType = chain.InputType(),
                    ResourceType = chain.ResourceType()
                });
            });

            return list;
        }
    }
}