using System;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;

namespace FubuMVC.Core.Security.AntiForgery
{
    public class AntiForgerySettings
    {
        private ChainPredicate _filter;

        public bool Enabled { get; set; }
        
        public AntiForgerySettings()
        {
            AppliesTo(filter => filter.RespondsToHttpMethod("POST")
                        .And
                        .ChainMatches(x => x.InputType() != null));
        }

        public void AppliesTo(Action<ChainPredicate> configuration)
        {
            _filter = new ChainPredicate();
            configuration(_filter);
        }


        public string Path { get; set; }
        public string Domain { get; set; }
       
        public bool AppliesTo(BehaviorChain chain)
        {
            return _filter.As<IChainFilter>().Matches(chain);
        }
    }
}