using System.Linq;
using Bottles;
using FubuMVC.Core.Registration;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Core.UI.Testing
{
    [TestFixture]
    public class UIServiceRegistry_registration_spec
    {
        private void registeredTypeIs<TService, TImplementation>()
        {
            var graph = buildGraph();
            graph.Services.DefaultServiceFor<TService>().Type.ShouldEqual(
                typeof(TImplementation));
        }

        private static BehaviorGraph buildGraph()
        {
            BehaviorGraph graph = BehaviorGraph.BuildFrom(x => x.Import<FubuHtmlRegistration>());
            return graph;
        }

        [Test]
        public void an_activator_for_HtmlConventionActivator_is_registered()
        {
            buildGraph().Services.ServicesFor<IActivator>()
                .Any(x => x.Type == typeof (HtmlConventionsActivator)).ShouldBeTrue();
        }


        [Test]
        public void registers_the_display_conversion_registry_activator()
        {
            buildGraph().Services.ServicesFor(typeof(IActivator))
                .Any(x => x.Type == typeof (DisplayConversionRegistryActivator));
        }

        [Test]
        public void partial_invoker_is_registered()
        {
            registeredTypeIs<IPartialInvoker, PartialInvoker>();
        }
    }
}