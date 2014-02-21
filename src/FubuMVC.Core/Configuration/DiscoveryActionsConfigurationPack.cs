using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;

namespace FubuMVC.Core.Configuration
{
    public class DiscoveryActionsConfigurationPack : ConfigurationPack
    {
        public DiscoveryActionsConfigurationPack()
        {
            For(ConfigurationType.Discovery);

            Add<PartialOnlyConvention>();
            Add<RouteDetermination>();
        }
    }
}