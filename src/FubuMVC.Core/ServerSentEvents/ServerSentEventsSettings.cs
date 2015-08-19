using FubuMVC.Core.Registration;

namespace FubuMVC.Core.ServerSentEvents
{
    public class ServerSentEventsSettings : IFeatureSettings
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
    }
}