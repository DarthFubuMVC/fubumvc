using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Bugs
{
    [TestFixture]
    public class Issue504
    {
        [Test]
        public void the_url_prefix_of_a_fubu_package_registry_should_be_respected()
        {
            var registry = new FubuRegistry();
            registry.Import<FooPackageRegistry>();

            var graph = BehaviorGraph.BuildFrom(registry);

            graph.BehaviorFor<FooEndpointClass>(x => x.get_some_foo())
                .As<RoutedChain>()
                 .GetRoutePattern().ShouldEqual("moar/some/foo");
        }
    }

    public class FooPackageRegistry : FubuPackageRegistry
    {
        public FooPackageRegistry() : base("moar")
        {
            Actions.IncludeType<FooEndpointClass>();
        }
    }

    public class FooEndpointClass
    {
        public string get_some_foo()
        {
            return "what?";
        }
    }
}