using FubuMVC.Core.Registration;

namespace FubuMVC.Core.View.Activation
{
    public class ViewActivationServiceRegistry : ServiceRegistry
    {
        public ViewActivationServiceRegistry()
        {
            SetServiceIfNone<IPageActivationRules, PageActivationRuleCache>();
            SetServiceIfNone<IPageActivator, PageActivator>();
        }
    }
}