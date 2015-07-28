using System;
using System.Runtime.Remoting.Messaging;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;

namespace FubuMVC.Core.Security.AntiForgery
{
    public class AntiForgerySettings : IFeatureSettings
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

        void IFeatureSettings.Apply(FubuRegistry registry)
        {
            if (!Enabled) return;

            registry.Services.IncludeRegistry<AntiForgeryServiceRegistry>();
            registry.Policies.Global.Add<AntiForgeryPolicy>();

        }
    }
}