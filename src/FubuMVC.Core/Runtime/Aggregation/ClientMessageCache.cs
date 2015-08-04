using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Runtime.Aggregation
{
    public class ClientMessageCache : IClientMessageCache
    {
        private readonly Cache<string, BehaviorChain> _chains = new Cache<string, BehaviorChain>();

        public ClientMessageCache(BehaviorGraph graph)
        {
            graph.Chains.Where(x => BehaviorChainExtensions.IsAggregatedChain(x)).Each(x =>
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
            if (!_chains.Has(messageName))
            {
                throw new ArgumentOutOfRangeException("No client message with name '{0}' exists in the system".ToFormat(messageName));
            }

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
                    ResourceType = chain.ResourceType(),
                    Chain = chain
                });
            });

            return list;
        }
    }
}