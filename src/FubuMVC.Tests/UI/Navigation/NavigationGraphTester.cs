using System.Linq;
using FubuMVC.Core.UI.Navigation;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.UI.Navigation
{
    [TestFixture]
    public class NavigationGraphTester
    {
        [Test]
        public void add_node_and_parent_does_not_exist()
        {
            var graph = new NavigationGraph();

            var node = MenuNode.Node(FakeKeys.Key1);

            graph.AddNode(FakeKeys.Key2, node);

            graph.MenuFor(FakeKeys.Key2).Top
                .ShouldBeTheSameAs(node);
        }

        [Test]
        public void add_node_to_another_node()
        {
            var graph = new NavigationGraph();
            var parentNode = MenuNode.Node(FakeKeys.Key1);
            graph.AddNode(FakeKeys.Key2, parentNode);


            var childNode = MenuNode.Node(FakeKeys.Key3);

            graph.AddNode(FakeKeys.Key1, childNode);

            parentNode.Children.Top
                .ShouldBeTheSameAs(childNode);
        }

    }

    [TestFixture]
    public class NavigationRegistryAndGraphTester
    {
        private NavigationGraph graph;

        [SetUp]
        public void SetUp()
        {
            graph = new NavigationGraph(x =>
            {
                x.ForMenu(FakeKeys.Key1);
                x.Add += MenuNode.Node(FakeKeys.Key2);
                x.Add += MenuNode.Node(FakeKeys.Key3);
                x.Add += MenuNode.Node(FakeKeys.Key4);


                x.ForMenu(FakeKeys.Key5);
                x.Add += MenuNode.Node(FakeKeys.Key6);
                x.Add += MenuNode.Node(FakeKeys.Key7);


                x.ForMenu(FakeKeys.Key2);
                x.Add += MenuNode.Node(FakeKeys.Key8);
                x.Add += MenuNode.Node(FakeKeys.Key9);

                x.ForMenu(FakeKeys.Key9);
                x.Add += MenuNode.Node(FakeKeys.Key10);
            });
        }

        [Test]
        public void has_all_the_menu_chain_keys()
        {
            graph.AllMenus().Select(x => x.Key)
                .ShouldHaveTheSameElementsAs(FakeKeys.Key1, FakeKeys.Key5);
                
        }

        [Test]
        public void has_all_the_nodes()
        {
            graph.AllNodes().Select(x => x.Key)
                .ShouldHaveTheSameElementsAs(FakeKeys.Key2, FakeKeys.Key8, FakeKeys.Key9, FakeKeys.Key10, FakeKeys.Key3, FakeKeys.Key4, FakeKeys.Key6, FakeKeys.Key7);
        }

        [Test]
        public void find_node()
        {
            graph.FindNode(FakeKeys.Key10).Key.ShouldEqual(FakeKeys.Key10);
            graph.FindNode(FakeKeys.Key8).Key.ShouldEqual(FakeKeys.Key8);
            graph.FindNode(FakeKeys.Key4).Key.ShouldEqual(FakeKeys.Key4);
        }
    }
}