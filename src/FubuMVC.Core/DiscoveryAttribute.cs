namespace FubuMVC.Core
{
    public class DiscoveryAttribute : ConfigurationTypeAttribute
    {
        public DiscoveryAttribute()
            : base(ConfigurationType.Discovery)
        {
        }
    }
}