using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;

namespace FubuMVC.Core
{
    public class DiscoveryActionsConfigurationPack : ConfigurationPack
    {
        public DiscoveryActionsConfigurationPack()
        {
            // Settings
            Add<AccessorOverridesFinder>();

            For(ConfigurationType.Discovery);

            Add<PartialOnlyConvention>();
            Add<RouteDetermination>();
        }
    }
}