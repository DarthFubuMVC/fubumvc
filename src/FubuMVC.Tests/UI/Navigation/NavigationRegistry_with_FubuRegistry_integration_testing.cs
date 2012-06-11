using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.UI.Navigation;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.UI.Navigation
{
    [TestFixture]
    public class NavigationRegistry_with_FubuRegistry_integration_testing
    {
        [Test]
        public void navigation_method_on_fubu_registry_works()
        {
            var registry = new FubuRegistry();
            registry.Navigation(x =>
            {
                x.ForMenu(FakeKeys.Key1);
                x.Add += MenuNode.Node(FakeKeys.Key2);
                x.Add += MenuNode.Node(FakeKeys.Key3);
            });

            var graph = BehaviorGraph.BuildFrom(registry).Navigation;

            graph.MenuFor(FakeKeys.Key1).Select(x => x.Key)
                .ShouldHaveTheSameElementsAs(FakeKeys.Key2, FakeKeys.Key3);
        }

        [Test]
        public void import_navigation_from_child_registry()
        {
            var registry = new FubuRegistry();
            registry.Navigation(x =>
            {
                x.ForMenu(FakeKeys.Key1);
                x.Add += MenuNode.Node(FakeKeys.Key2);
                x.Add += MenuNode.Node(FakeKeys.Key3);
            });

            registry.Import<ChildRegistry>();

            var graph = BehaviorGraph.BuildFrom(registry).Navigation;

            graph.MenuFor(FakeKeys.Key1).Select(x => x.Key)
                .ShouldHaveTheSameElementsAs(FakeKeys.Key2, FakeKeys.Key3, FakeKeys.Key4, FakeKeys.Key5);

            graph.MenuFor(FakeKeys.Key6).Select(x => x.Key)
                .ShouldHaveTheSameElementsAs(FakeKeys.Key7, FakeKeys.Key8);
        }

        public class ChildRegistry : FubuPackageRegistry
        {
            public ChildRegistry()
            {
                Navigation<ChildNavigation>();
            }
        }

        public class ChildNavigation : NavigationRegistry
        {
            public ChildNavigation()
            {
                ForMenu(FakeKeys.Key1);
                Add += MenuNode.Node(FakeKeys.Key4);
                Add += MenuNode.Node(FakeKeys.Key5);

                ForMenu(FakeKeys.Key6);
                Add += MenuNode.Node(FakeKeys.Key7);
                Add += MenuNode.Node(FakeKeys.Key8);
            }
        }
    }
}