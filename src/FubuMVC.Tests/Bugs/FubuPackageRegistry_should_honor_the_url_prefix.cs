using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using FubuCore;
using FubuTestingSupport;

namespace FubuMVC.Tests.Bugs
{
    [TestFixture]
    public class FubuPackageRegistry_should_honor_the_url_prefix
    {
        [Test]
        public void the_routes_from_FubuPackageRegistry_have_the_prefix()
        {
            var registry = new FubuRegistry();
            new SomePackageRegistry().As<IFubuRegistryExtension>().Configure(registry);

            var graph = BehaviorGraph.BuildFrom(registry);

            graph.BehaviorFor<SomeRandomClass>(x => x.get_some_data())
                .As<RoutedChain>()
                .GetRoutePattern().ShouldStartWith("mypak");
        }
    }

    public class SomePackageRegistry : FubuPackageRegistry
    {
        public SomePackageRegistry() : base("mypak")
        {
            Actions.IncludeType<SomeRandomClass>();
        }
    }

    public class SomeRandomClass
    {
        public string get_some_data()
        {
            return "Hello";
        }
    }
}