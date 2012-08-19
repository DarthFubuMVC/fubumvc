using System.Linq;
using Bottles;
using FubuMVC.Core.Registration;
using FubuMVC.Core.UI;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class UIServiceRegistry_registration_spec
    {
        private void registeredTypeIs<TService, TImplementation>()
        {
            BehaviorGraph.BuildEmptyGraph().Services.DefaultServiceFor<TService>().Type.ShouldEqual(
                typeof(TImplementation));
        }

        [Test]
        public void an_activator_for_HtmlConventionActivator_is_registered()
        {
            BehaviorGraph.BuildEmptyGraph().Services.ServicesFor<IActivator>()
                .Any(x => x.Type == typeof (HtmlConventionsActivator)).ShouldBeTrue();
        }


        [Test]
        public void registers_the_display_conversion_registry_activator()
        {
            BehaviorGraph.BuildEmptyGraph().Services.ServicesFor(typeof (IActivator))
                .Any(x => x.Type == typeof (DisplayConversionRegistryActivator));
        }

        [Test]
        public void partial_invoker_is_registered()
        {
            registeredTypeIs<IPartialInvoker, PartialInvoker>();
        }
    }
}