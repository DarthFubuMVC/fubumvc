using System;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Security.AntiForgery
{
    public class AntiForgerySettings : IFeatureSettings, DescribesItself
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

        public void Describe(Description description)
        {
            description.Properties[nameof(Enabled)] = Enabled.ToString();
            description.Properties[nameof(Path)] = Path;
            description.Properties[nameof(Domain)] = Domain;
        }
    }
}