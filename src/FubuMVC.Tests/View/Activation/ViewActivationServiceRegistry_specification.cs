using FubuMVC.Core.Registration;
using FubuMVC.Core.View.Activation;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.View.Activation
{
    [TestFixture]
    public class ViewActivationServiceRegistry_specification
    {
        private void registeredTypeIs<TService, TImplementation>()
        {
            BehaviorGraph.BuildEmptyGraph().Services.DefaultServiceFor<TService>().Type.ShouldEqual(
                typeof(TImplementation));
        }

        [Test]
        public void page_activation_rule_cache_is_registered()
        {
            registeredTypeIs<IPageActivationRules, PageActivationRuleCache>();
        }

        [Test]
        public void page_activator_is_registered()
        {
            registeredTypeIs<IPageActivator, PageActivator>();
        }
    }
}