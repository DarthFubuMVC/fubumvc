using FubuCore.Descriptions;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.ServerSentEvents
{
    public class ServerSentEventsSettings : IFeatureSettings, DescribesItself
    {
        public bool Enabled { get; set; }

        void IFeatureSettings.Apply(FubuRegistry registry)
        {
            if (Enabled)
            {
                registry.Services.IncludeRegistry<ServerSentEventRegistry>();

                registry.Policies.ChainSource<TopicChainSource>();
            }
        }

        public void Describe(Description description)
        {
            description.Properties[nameof(Enabled)] = Enabled.ToString();
        }
    }
}