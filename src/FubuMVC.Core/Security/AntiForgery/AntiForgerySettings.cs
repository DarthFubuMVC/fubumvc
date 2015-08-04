using System;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Security.AntiForgery
{
    public class AntiForgerySettings : IFeatureSettings
    {
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