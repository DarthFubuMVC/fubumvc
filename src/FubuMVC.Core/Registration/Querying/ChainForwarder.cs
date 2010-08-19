using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Querying
{
    public delegate IEnumerable<BehaviorChain> Forwardable(object model, IChainResolver resolver);

    public class ChainForwarder
    {
        private readonly Forwardable _forwarding;

        public ChainForwarder(Type inputType, Forwardable forwarding)
        {
            _forwarding = forwarding;
            InputType = inputType;
        }

        public Type InputType { get; private set; }
        public string Category { get; set; }

        public IEnumerable<BehaviorChain> Find(IChainResolver resolver, object model)
        {
            return _forwarding(model, resolver);
        }
    }
}