using FubuMVC.Core;
using FubuMVC.Core.Registration;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Navigation.Testing
{
    [TestFixture]
    public class NavigationRegistry_with_FubuRegistry_integration_testing
    {
        [Test]
        public void navigation_method_on_fubu_registry_works()
        {
            var registry = new FubuRegistry();
            registry.Import<NavigationRegistryExtension>();

            registry.Policies.Add<NavigationRegistry>(x =>
            {
                x.ForMenu(FakeKeys.Key1);
                x.Add += MenuNode.Node(FakeKeys.Key2);
                x.Add += MenuNode.Node(FakeKeys.Key3);
            });

            var graph = BehaviorGraph.BuildFrom(registry).Settings.Get<NavigationGraph>();

            graph.MenuFor(FakeKeys.Key1).Select(x => x.Key)
                .ShouldHaveTheSameElementsAs(FakeKeys.Key2, FakeKeys.Key3);
        }

        [Test]
        public void import_navigation_from_child_registry()
        {
            var registry = new FubuRegistry();
            registry.Import<NavigationRegistryExtension>();

            registry.Policies.Add<NavigationRegistry>(x =>
            {
                x.ForMenu(FakeKeys.Key1);
                x.Add += MenuNode.Node(FakeKeys.Key2);
                x.Add += MenuNode.Node(FakeKeys.Key3);
            });

            registry.Import<ChildRegistry>();

            var graph = BehaviorGraph.BuildFrom(registry).Settings.Get<NavigationGraph>();

            graph.MenuFor(FakeKeys.Key1).Select(x => x.Key)
                .ShouldHaveTheSameElementsAs(FakeKeys.Key2, FakeKeys.Key3, FakeKeys.Key4, FakeKeys.Key5);

            graph.MenuFor(FakeKeys.Key6).Select(x => x.Key)
                .ShouldHaveTheSameElementsAs(FakeKeys.Key7, FakeKeys.Key8);
        }

        [Test]
        public void import_with_strings_instead_of_StringToken()
        {
            var registry = new FubuRegistry();
            registry.Import<NavigationRegistryExtension>();

            registry.Policies.Add<NavigationRegistry>(x =>
            {
                x.ForMenu("Key1");
                x.Add += MenuNode.Node("Key2");
                x.Add += MenuNode.Node("Key3");
            });

            registry.Import<ChildRegistry>();

            var graph = BehaviorGraph.BuildFrom(registry).Settings.Get<NavigationGraph>();

            graph.MenuFor("Key1").Select(x => x.Key)
                .ShouldHaveTheSameElementsAs(new NavigationKey("Key2"), new NavigationKey("Key3"));
        }

        public class ChildRegistry : FubuPackageRegistry
        {
            public ChildRegistry()
            {
                Policies.Add<ChildNavigation>();
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