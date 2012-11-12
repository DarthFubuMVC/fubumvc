using System;
using FubuCore;
using FubuLocalization;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Runtime.Conditionals;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Navigation.Testing
{
    [TestFixture]
    public class MenuNodeTester
    {
        [Test]
        public void set_the_icon()
        {
            var node = new MenuNode(FakeKeys.Key9);
            node.Icon("something.png").ShouldBeTheSameAs(node);

            node.Icon().ShouldEqual("something.png");
        }

        [Test]
        public void un_authorized_state_is_hidden_by_default()
        {
            MenuNode.ForCreatorOf<Address>(StringToken.FromKeyString("something"))
                .UnauthorizedState.ShouldEqual(MenuItemState.Hidden);
        }

        [Test]
        public void positive_case_of_setting_is_enabled_type()
        {
            var node = MenuNode.ForCreatorOf<Address>(StringToken.FromKeyString("something"));
            node.IsEnabledBy(typeof (FakeConditional));

            node.IsEnabledBy().ShouldEqual(typeof (FakeConditional));
        }

        [Test]
        public void negative_case_of_setting_is_enabled_type()
        {
            var node = MenuNode.ForCreatorOf<Address>(StringToken.FromKeyString("something"));
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                node.IsEnabledBy(GetType());
            });
        }

        [Test]
        public void positive_case_of_setting_hide_if_type()
        {
            var node = MenuNode.ForCreatorOf<Address>(StringToken.FromKeyString("something"));
            node.HideIf(typeof(FakeConditional));

            node.HideIfConditional.ShouldEqual(typeof(FakeConditional));
        }

        [Test]
        public void negative_case_of_setting_hide_if_type()
        {
            var node = MenuNode.ForCreatorOf<Address>(StringToken.FromKeyString("something"));
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                node.HideIf(GetType());
            });
        }

        [Test]
        public void hide_if_is_never_by_default()
        {
            var node = MenuNode.ForCreatorOf<Address>(StringToken.FromKeyString("something"));
            node.HideIfConditional.ShouldEqual(typeof (Never));
        }

        private void resolve(MenuNode node, Action<BehaviorGraph> configure)
        {
            var graph = new BehaviorGraph();
            configure(graph);
            var resolver = new ChainResolutionCache(new TypeResolver(), graph);

            node.Resolve(resolver);
        }

        [Test]
        public void throws_if_the_chain_cannot_be_resolved_to_fail_fast()
        {
            var key = StringToken.FromKeyString("Something");
            var node = MenuNode.ForCreatorOf(key, typeof(Address));

            Exception<InvalidOperationException>.ShouldBeThrownBy(() =>
            {
                resolve(node, g => { });
            });

            
        }

        [Test]
        public void create_by_creator_of()
        {
            var key = StringToken.FromKeyString("Something");
            var node = MenuNode.ForCreatorOf(key, typeof (Address));

            
            var chain = new BehaviorChain();
            chain.UrlCategory.Creates.Add(typeof(Address));

            resolve(node, graph => graph.AddChain(chain));


            node.BehaviorChain.ShouldBeTheSameAs(chain);
        }

        [Test]
        public void create_for_action()
        {
            var key = StringToken.FromKeyString("Something");
            var node = MenuNode.ForAction<FakeController>(key, x => x.GetFake());

            var chain1 = new BehaviorChain();
            chain1.AddToEnd(ActionCall.For<FakeController>(x => x.GetFake()));
            chain1.Route = new RouteDefinition("something");
            chain1.Route.AddHttpMethodConstraint("GET");

            var chain2 = new BehaviorChain();
            chain2.AddToEnd(ActionCall.For<FakeController>(x => x.GetFake()));
            chain2.Route = new RouteDefinition("something");
            chain2.Route.AddHttpMethodConstraint("POST");

            resolve(node, graph =>
            {
                graph.AddChain(chain1);
                graph.AddChain(chain2);
                graph.AddChain(new BehaviorChain());
                graph.AddChain(new BehaviorChain());
                graph.AddChain(new BehaviorChain());
                graph.AddChain(new BehaviorChain());
            });

            node.BehaviorChain.ShouldBeTheSameAs(chain1);
        }

        [Test]
        public void create_for_input_where_it_is_null()
        {
            var key = StringToken.FromKeyString("Something");
            var node = MenuNode.ForInput<FakeInput>(key);

            node.UrlInput.ShouldBeNull();

            var chain1 = new BehaviorChain();
            chain1.AddToEnd(ActionCall.For<FakeController>(x => x.FromInput(null)));
            chain1.Route = new RouteDefinition("something");
            chain1.Route.AddHttpMethodConstraint("GET");

            var chain2 = new BehaviorChain();
            chain2.AddToEnd(ActionCall.For<FakeController>(x => x.FromInput(null)));
            chain2.Route = new RouteDefinition("something");
            chain2.Route.AddHttpMethodConstraint("POST");

            resolve(node, graph =>
            {
                graph.AddChain(chain1);
                graph.AddChain(chain2);
            });

            node.BehaviorChain.ShouldBeTheSameAs(chain1);
        }

        [Test]
        public void for_intput_with_model()
        {
            var key = StringToken.FromKeyString("Something");
            var input = new FakeInput(){Name = "something"};
            var node = MenuNode.ForInput(key, input);

            node.UrlInput.ShouldBeTheSameAs(input);

            var chain1 = new BehaviorChain();
            chain1.AddToEnd(ActionCall.For<FakeController>(x => x.FromInput(null)));
            chain1.Route = new RouteDefinition("something");
            chain1.Route.AddHttpMethodConstraint("GET");

            var chain2 = new BehaviorChain();
            chain2.AddToEnd(ActionCall.For<FakeController>(x => x.FromInput(null)));
            chain2.Route = new RouteDefinition("something");
            chain2.Route.AddHttpMethodConstraint("POST");

            resolve(node, graph =>
            {
                graph.AddChain(chain1);
                graph.AddChain(chain2);
            });

            node.BehaviorChain.ShouldBeTheSameAs(chain1);
        }

        [Test]
        public void create_by_chain_by_itself()
        {
            var key = StringToken.FromKeyString("Something");
            var chain = new BehaviorChain();
            var node = MenuNode.ForChain(key, chain);

            node.BehaviorChain.ShouldBeTheSameAs(chain);
        }

        [Test]
        public void menu_type_when_it_is_just_a_node()
        {
            var node = new MenuNode(FakeKeys.Key5);
            node.Type.ShouldEqual(MenuNodeType.Node);

            // doesn't blow up;)
            node.Resolve(null);
        }

        [Test]
        public void menu_type_when_it_is_a_leaf()
        {
            var node = MenuNode.ForCreatorOf(FakeKeys.Key1, typeof (Address));
            node.Type.ShouldEqual(MenuNodeType.Leaf);
        }

        [Test]
        public void find_all_children()
        {
            var node1 = MenuNode.ForCreatorOf(FakeKeys.Key1, typeof(Address));
            var node2 = MenuNode.ForCreatorOf(FakeKeys.Key2, typeof(Address));
            var node3 = MenuNode.ForCreatorOf(FakeKeys.Key3, typeof(Address));
            var node4 = MenuNode.ForCreatorOf(FakeKeys.Key4, typeof(Address));
            var node5 = MenuNode.ForCreatorOf(FakeKeys.Key5, typeof(Address));
            var node6 = MenuNode.ForCreatorOf(FakeKeys.Key6, typeof(Address));
            var node7 = MenuNode.ForCreatorOf(FakeKeys.Key7, typeof(Address));
            var node8 = MenuNode.ForCreatorOf(FakeKeys.Key8, typeof(Address));

            node1.Children.AddToEnd(node2);
            node1.Children.AddToEnd(node3);

            node2.Children.AddToEnd(node4);
            node4.Children.AddToEnd(node5);

            node3.Children.AddToEnd(node6);
            node3.Children.AddToEnd(node7);
            node3.Children.AddToEnd(node8);

            node1.FindAllChildren().ShouldHaveTheSameElementsAs(node2, node4, node5, node3, node6, node7, node8);
        }


    }


    public class FakeKeys : StringToken
    {
        public static readonly FakeKeys Key1 = new FakeKeys("1");
        public static readonly FakeKeys Key2 = new FakeKeys("2");
        public static readonly FakeKeys Key3 = new FakeKeys("3");
        public static readonly FakeKeys Key4 = new FakeKeys("4");
        public static readonly FakeKeys Key5 = new FakeKeys("5");
        public static readonly FakeKeys Key6 = new FakeKeys("6");
        public static readonly FakeKeys Key7 = new FakeKeys("7");
        public static readonly FakeKeys Key8 = new FakeKeys("8");
        public static readonly FakeKeys Key9 = new FakeKeys("9");
        public static readonly FakeKeys Key10 = new FakeKeys("10");
        public static readonly FakeKeys Key11 = new FakeKeys("11");
        public static readonly FakeKeys Chain1 = new FakeKeys("Chain1");
        public static readonly FakeKeys Chain2 = new FakeKeys("Chain2");
        public static readonly FakeKeys Chain3 = new FakeKeys("Chain3");

        protected FakeKeys(string defaultValue) : base(null, defaultValue, namespaceByType:true)
        {
        }
    }

    public class FakeController
    {
        public string GetFake()
        {
            return "get fake";
        }

        public string FromInput(FakeInput input)
        {
            return input.Name;
        }
    }

    public class FakeInput
    {
        public string Name { get; set;}
    }

    public class FakeConditional : IConditional
    {
        public bool ShouldExecute()
        {
            throw new NotImplementedException();
        }
    }
}