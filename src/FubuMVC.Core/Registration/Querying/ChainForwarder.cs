using System;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Querying
{
    public class ChainForwarder
    {
        private readonly Func<object, IChainResolver, BehaviorChain> _forwarding;

        public ChainForwarder(Type inputType, Func<object, IChainResolver, BehaviorChain> forwarding)
        {
            _forwarding = forwarding;
            InputType = inputType;
        }

        public Type InputType { get; private set; }
        public string Category { get; set; }

        public BehaviorChain FindChain(IChainResolver resolver, object model)
        {
            throw new NotImplementedException();
        }
    }
}