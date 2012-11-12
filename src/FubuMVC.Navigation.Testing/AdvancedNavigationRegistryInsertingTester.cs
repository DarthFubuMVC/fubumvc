using System;
using FubuMVC.Core.Registration;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Navigation.Testing
{
    [TestFixture]
    public class AdvancedNavigationRegistryInsertingTester
    {
        private Lazy<NavigationGraph> _graph = null;
        private NavigationRegistry registry;

        [SetUp]
        public void SetUp()
        {
            registry = new NavigationRegistry();
            _graph = new Lazy<NavigationGraph>(() =>
            {
                return BehaviorGraph.BuildFrom(x =>
                {
                    x.Import<NavigationRegistryExtension>();
                    x.Actions.IncludeType<Controller1>();
                    x.Policies.Add(registry);
                }).Settings.Get<NavigationGraph>();
            });
        }

        private NavigationGraph theGraph
        {
            get
            {
                return _graph.Value;
            }
        }

        [Test]
        public void insert_after_by_string()
        {
            registry.ForMenu("Root");
            registry.Add += MenuNode.Node("One");
            registry.InsertAfter["One"] = MenuNode.Node("Two");

            theGraph.MenuFor("Root").Select(x => x.Key.Key)
                .ShouldHaveTheSameElementsAs("One", "Two");
        }

        [Test]
        public void insert_after_by_token()
        {
            registry.ForMenu("Root");
            registry.Add += MenuNode.Node(FakeKeys.Key1);
            registry.InsertAfter[FakeKeys.Key1] = MenuNode.Node(FakeKeys.Key2);

            theGraph.MenuFor("Root").Select(x => x.Key.Key)
                .ShouldHaveTheSameElementsAs("Key1", "Key2");
        }

        [Test]
        public void insert_after_by_mixed()
        {
            registry.ForMenu("Root");
            registry.Add += MenuNode.Node(FakeKeys.Key1);
            registry.InsertAfter[FakeKeys.Key1.ToLocalizationKey().ToString()] = MenuNode.Node(FakeKeys.Key2);

            theGraph.MenuFor("Root").Select(x => x.Key.Key)
                .ShouldHaveTheSameElementsAs("Key1", "Key2");
        }

        [Test]
        public void insert_before_by_string()
        {
            registry.InsertBefore["Two"] = MenuNode.Node("One");

            registry.ForMenu("Root");
            registry.Add += MenuNode.Node("Two");

            theGraph.MenuFor("Root").Select(x => x.Key.Key)
                .ShouldHaveTheSameElementsAs("One", "Two");
        }

        [Test]
        public void insert_before_by_token()
        {
            registry.InsertBefore["Two"] = MenuNode.Node(FakeKeys.Key9);

            registry.ForMenu("Root");
            registry.Add += MenuNode.Node("Two");

            theGraph.MenuFor("Root").Select(x => x.Key.Key)
                .ShouldHaveTheSameElementsAs(FakeKeys.Key9.Key, "Two");
        }
        
        public class Controller1
        {
            public void One(Input1 input) { }

            public void Two(Input1 input) { }

            public void Three(Input1 input) { }

            public void Four(Input1 input) { }


            public class Input1 { }
        }
    }
}