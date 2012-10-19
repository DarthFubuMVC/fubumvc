using FubuCore;

namespace FubuMVC.Core
{
    [MarkedForTermination]
    public class DiscoveryAttribute : ConfigurationTypeAttribute
    {
        public DiscoveryAttribute()
            : base(ConfigurationType.Discovery)
        {
        }
    }
}