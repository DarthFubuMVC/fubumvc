using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;

namespace FubuMVC.Core
{
    public class DiscoveryActionsConfigurationPack : ConfigurationPack
    {
        public DiscoveryActionsConfigurationPack()
        {
            For(ConfigurationType.Discovery);

            Add<PartialOnlyConvention>();
            Add<RouteDetermination>();
            Add<AccessorOverridesFinder>();
        }
    }
}