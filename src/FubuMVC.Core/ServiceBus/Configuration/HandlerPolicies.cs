using FubuMVC.Core.Registration;

namespace FubuMVC.Core.ServiceBus.Configuration
{
    [ApplicationLevel]
    public class HandlerPolicies
    {
        private readonly ConfigurationActionSet _globalPolicies = new ConfigurationActionSet();

        public void AddGlobal(IConfigurationAction action, FubuTransportRegistry registry)
        {
            _globalPolicies.Fill(action);
        }

        public ConfigurationActionSet GlobalPolicies
        {
            get { return _globalPolicies; }
        }
    }
}