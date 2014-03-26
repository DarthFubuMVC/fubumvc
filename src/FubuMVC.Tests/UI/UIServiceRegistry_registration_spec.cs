using System.Linq;
using Bottles;
using FubuMVC.Core.Registration;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Templates;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class UIServiceRegistry_registration_spec
    {
        private BehaviorGraph graph = buildGraph();

        private void registeredTypeIs<TService, TImplementation>()
        {
            graph.Services.DefaultServiceFor<TService>().Type.ShouldEqual(
                typeof(TImplementation));
        }

        private static BehaviorGraph buildGraph()
        {
            BehaviorGraph graph = BehaviorGraph.BuildEmptyGraph();
            return graph;
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

        [Test]
        public void ITemplateWriter_is_registered()
        {
            registeredTypeIs<ITemplateWriter, TemplateWriter>();
        }
    }
}