using System;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Querying;

namespace FubuMVC.Core.UI.Navigation
{
    // TODO -- need this registered!
    [ConfigurationType(ConfigurationType.Navigation)]
    public class MenuItemAttributeConfigurator : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            throw new NotImplementedException();
        }
    }
}