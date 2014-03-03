using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;

namespace FubuMVC.Core.Configuration
{
    public class DiscoveryActionsConfigurationPack : ConfigurationPack
    {
        public DiscoveryActionsConfigurationPack()
        {
            For(ConfigurationType.Explicit);

            Add<PartialOnlyConvention>();
            Add<RouteDetermination>();
        }
    }
}