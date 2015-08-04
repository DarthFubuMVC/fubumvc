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
        
        public Func<RoutedChain, bool> Matches = c => c.MatchesCategoryOrHttpMethod("POST") && c.InputType() != null; 


        public string Path { get; set; }
        public string Domain { get; set; }

        void IFeatureSettings.Apply(FubuRegistry registry)
        {
            if (!Enabled) return;

            registry.Services.IncludeRegistry<AntiForgeryServiceRegistry>();
            registry.Policies.Local.Add<AntiForgeryPolicy>();

        }
    }
}