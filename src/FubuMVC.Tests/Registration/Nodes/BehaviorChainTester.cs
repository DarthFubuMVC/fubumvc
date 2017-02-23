using System;
using System.Linq;
using FubuMVC.Core.Json;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using Shouldly;
using Xunit;
using StructureMap.Pipeline;
using TestPackage1.FakeControllers;

namespace FubuMVC.Tests.Registration.Nodes
{
    public class StubNode : BehaviorNode
    {
        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Process; }
        }

        protected override IConfiguredInstance buildInstance()
        {
            throw new NotImplementedException();
        }
    }

    
    public class when_moving_a_node_to_first_in_the_chain
    {
        public when_moving_a_node_to_first_in_the_chain()
        {
            theChain = new BehaviorChain();

            node1 = new StubNode();
            node2 = new StubNode();
            node3 = new StubNode();

            theChain.AddToEnd(node1);
            theChain.AddToEnd(node2);
            theChain.AddToEnd(node3);

            theChain.ShouldHaveTheSameElementsAs(node1, node2, node3);
        }


        private BehaviorChain theChain;
        private StubNode node1;
        private StubNode node2;
        private StubNode node3;

        [Fact]
        public void move_first_1()
        {
            node1.MoveToFront();

            // No change, but I don't want it blowing up either
            theChain.ShouldHaveTheSameElementsAs(node1, node2, node3);
        }

        [Fact]
        public void move_first_2()
        {
            node2.MoveToFront();

            theChain.ShouldHaveTheSameElementsAs(node2, node1, node3);
        }

        [Fact]
        public void move_first_3()
        {
            node3.MoveToFront();

            theChain.ShouldHaveTheSameElementsAs(node3, node1, node2);
        }
    }


    
    public class BehaviorChainTester
    {
        [Fact]
        public void adding_a_node_to_the_end_sets_the_chain_on_the_node()
        {
            var chain = new BehaviorChain();
            var wrapper = new Wrapper(typeof (FakeJsonBehavior));

            chain.AddToEnd(wrapper);

            wrapper.ParentChain().ShouldBeTheSameAs(chain);
        }


        [Fact]
        public void add_a_route_alias()
        {
            var chain = new RoutedChain("something/else");
            var alias = new RouteDefinition("something/else");

            chain.AddRouteAlias(alias);

            chain.AdditionalRoutes.ShouldHaveTheSameElementsAs(alias);
        }

        [Fact]
        public void key()
        {
            var chain = new RoutedChain("something/else");
            chain.Key.ShouldBe(chain.Title().GetHashCode());
        }

        [Fact]
        public void append_with_no_behaviors()
        {
            var chain = new BehaviorChain();
            var wrapper = new Wrapper(typeof (FakeJsonBehavior));

            chain.AddToEnd(wrapper);

            chain.Top.ShouldBeTheSameAs(wrapper);
        }

        [Fact]
        public void appending_a_node_also_sets_the_previous()
        {
            var chain = new BehaviorChain();
            var wrapper = new Wrapper(typeof (FakeJsonBehavior));

            chain.AddToEnd(wrapper);

            wrapper.Previous.ShouldBeNull();

            var wrapper2 = new Wrapper(typeof (FakeJsonBehavior));

            wrapper.AddToEnd(wrapper2);

            wrapper2.Previous.ShouldBeTheSameAs(wrapper);
            wrapper.Previous.ShouldBeNull();
        }


        [Fact]
        public void appending_a_node_also_sets_the_previous_2()
        {
            var chain = new BehaviorChain();
            var wrapper = new Wrapper(typeof (FakeJsonBehavior));

            chain.AddToEnd(wrapper);

            wrapper.Previous.ShouldBeNull();
            wrapper.ParentChain().ShouldBeTheSameAs(chain);

            var wrapper2 = new Wrapper(typeof (FakeJsonBehavior));

            chain.AddToEnd(wrapper2);

            wrapper2.Previous.ShouldBeTheSameAs(wrapper);
            wrapper.Previous.ShouldBeNull();
            wrapper.ParentChain().ShouldBeTheSameAs(chain);
        }

        [Fact]
        public void building_input_node_without_an_input_type_blows_up()
        {
            Exception<InvalidOperationException>.ShouldBeThrownBy(() => { new BehaviorChain().Input.ShouldBeNull(); });
        }

        [Fact]
        public void building_output_node_without_an_output_type_blows_up()
        {
            Exception<InvalidOperationException>.ShouldBeThrownBy(() => { new BehaviorChain().Output.ShouldBeNull(); });
        }

        [Fact]
        public void calls_finds_all_calls_underneath_the_chain()
        {
            var chain = new BehaviorChain();
            var call = ActionCall.For<TestController>(x => x.AnotherAction(null));
            var call2 = ActionCall.For<TestController>(x => x.AnotherAction(null));
            chain.AddToEnd(call);
            chain.AddToEnd(call2);

            chain.Calls.Count().ShouldBe(2);
            chain.Calls.Contains(call).ShouldBeTrue();
            chain.Calls.Contains(call2).ShouldBeTrue();
        }

        [Fact]
        public void find_the_chain_when_the_parent_is_null_should_be_null()
        {
            var node = new Wrapper(typeof (FakeJsonBehavior));
            node.ParentChain().ShouldBeNull();
        }

        [Fact]
        public void find_the_parent_chain_when_the_chain_is_the_immediate_parent()
        {
            var chain = new BehaviorChain();
            var node = new Wrapper(typeof (FakeJsonBehavior));
            chain.AddToEnd(node);
            node.ParentChain().ShouldBeTheSameAs(chain);
        }

        [Fact]
        public void find_the_parent_from_deep_in_behavior_chain()
        {
            var chain = new BehaviorChain();
            var node = new Wrapper(typeof (FakeJsonBehavior));
            chain.AddToEnd(node);

            var node2 = new Wrapper(typeof (FakeJsonBehavior));
            node.AddToEnd(node2);

            var node3 = new Wrapper(typeof (FakeJsonBehavior));
            node2.AddToEnd(node3);

            node3.ParentChain().ShouldBeTheSameAs(chain);
        }

        [Fact]
        public void has_input_depends_on_the_input_node_now()
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(ActionCall.For<OneController>(x => x.Query(null)));

            chain.HasReaders().ShouldBeFalse();

            chain.Input.Add(new NewtonsoftJsonFormatter());

            chain.HasReaders().ShouldBeTrue();
        }

        [Fact]
        public void has_input_initial()
        {
            new BehaviorChain().HasReaders()
                .ShouldBeFalse();
        }

        [Fact]
        public void has_output_depends_on_the_output_node_now()
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(ActionCall.For<OneController>(x => x.Query(null)));

            chain.HasOutput().ShouldBeFalse();

            chain.Output.Add(new NewtonsoftJsonFormatter());

            chain.HasOutput().ShouldBeTrue();
        }

        [Fact]
        public void has_output_initial()
        {
            new BehaviorChain().HasOutput()
                .ShouldBeFalse();
        }


        [Fact]
        public void insert_before_on_a_node()
        {
            var chain = new BehaviorChain();
            var wrapper = new Wrapper(typeof (FakeJsonBehavior));

            chain.AddToEnd(wrapper);

            var wrapper2 = new Wrapper(typeof (FakeJsonBehavior));

            wrapper.AddBefore(wrapper2);

            chain.Top.ShouldBeTheSameAs(wrapper2);
            wrapper2.Next.ShouldBeTheSameAs(wrapper);

            wrapper2.Previous.ShouldBeNull();
            wrapper.Previous.ShouldBeTheSameAs(wrapper2);
        }

        [Fact]
        public void lazy_creation_of_the_input_node_after_having_an_action()
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(ActionCall.For<OneController>(x => x.Query(null)));

            var o1 = chain.Input;
            var o2 = chain.Input;
            var o3 = chain.Input;

            o1.ShouldBeTheSameAs(o2);
            o1.ShouldBeTheSameAs(o3);
        }

        [Fact]
        public void lazy_creation_of_the_output_node_after_having_an_action()
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(ActionCall.For<OneController>(x => x.Query(null)));

            var o1 = chain.Output;
            var o2 = chain.Output;
            var o3 = chain.Output;

            o1.ShouldBeTheSameAs(o2);
            o1.ShouldBeTheSameAs(o3);
        }

        [Fact]
        public void prepend_with_an_existing_top_behavior()
        {
            var chain = new BehaviorChain();
            var call = ActionCall.For<TestController>(x => x.AnotherAction(null));
            chain.AddToEnd(call);

            var wrapper = new Wrapper(typeof (FakeJsonBehavior));
            chain.Prepend(wrapper);

            chain.Top.ShouldBeTheSameAs(wrapper);
            wrapper.Next.ShouldBeTheSameAs(call);
        }

        [Fact]
        public void prepend_with_no_behaviors()
        {
            var chain = new BehaviorChain();
            var wrapper = new Wrapper(typeof (FakeJsonBehavior));

            chain.Prepend(wrapper);

            chain.Top.ShouldBeTheSameAs(wrapper);
        }

        [Fact]
        public void removing_a_node_maintains_the_link_between_its_predecessor_and_successor()
        {
            var node1 = new Wrapper(typeof (FakeJsonBehavior));
            var node2 = new Wrapper(typeof (FakeJsonBehavior));
            var node3 = new Wrapper(typeof (FakeJsonBehavior));
            node1.AddToEnd(node2);
            node1.AddToEnd(node3);
            node2.Remove();

            node2.Previous.ShouldBeNull();
            node2.Next.ShouldBeNull();
            node1.Next.ShouldBeTheSameAs(node3);
            node3.Previous.ShouldBeTheSameAs(node1);
        }

        [Fact]
        public void removing_a_node_without_a_predecessor_sets_its_successor_to_the_front()
        {
            var node1 = new Wrapper(typeof (FakeJsonBehavior));
            var node2 = new Wrapper(typeof (FakeJsonBehavior));
            node1.AddToEnd(node2);

            node1.Remove();

            node1.Previous.ShouldBeNull();
            node1.Next.ShouldBeNull();
            node2.Previous.ShouldBeNull();
        }

        [Fact]
        public void replacing_a_node_should_disconnect_the_node_being_replaced()
        {
            var node1 = new Wrapper(typeof (FakeJsonBehavior));
            var node2 = new Wrapper(typeof (FakeJsonBehavior));
            var node3 = new Wrapper(typeof (FakeJsonBehavior));
            var newNode = new Wrapper(typeof (FakeJsonBehavior));
            node1.AddToEnd(node2);
            node1.AddToEnd(node3);
            node2.ReplaceWith(newNode);

            node2.Next.ShouldBeNull();
            node2.Previous.ShouldBeNull();
        }


        [Fact]
        public void replacing_a_node_should_set_the_new_nodes_predecessor_and_successor()
        {
            var node1 = new Wrapper(typeof (FakeJsonBehavior));
            var node2 = new Wrapper(typeof (FakeJsonBehavior));
            var node3 = new Wrapper(typeof (FakeJsonBehavior));
            var newNode = new Wrapper(typeof (FakeJsonBehavior));
            node1.AddToEnd(node2);
            node1.AddToEnd(node3);
            node2.ReplaceWith(newNode);

            newNode.Previous.ShouldBeTheSameAs(node1);
            newNode.Next.ShouldBeTheSameAs(node3);
            node1.Next.ShouldBeTheSameAs(newNode);
            node3.Previous.ShouldBeTheSameAs(newNode);
        }

        [Fact]
        public void replacing_a_node_without_a_predecessor_should_set_the_new_node_to_the_front()
        {
            var node1 = new Wrapper(typeof (FakeJsonBehavior));
            var node2 = new Wrapper(typeof (FakeJsonBehavior));
            var newNode = new Wrapper(typeof (FakeJsonBehavior));
            node1.AddToEnd(node2);
            node1.ReplaceWith(newNode);

            newNode.Previous.ShouldBeNull();
            newNode.Next.ShouldBeTheSameAs(node2);
        }
    }

    
    public class BehaviorChainMatchesCategoryOrHttpMethodTester
    {
        private RoutedChain theChain = new RoutedChain("something");

        [Fact]
        public void negative_on_category()
        {
            theChain.UrlCategory.Category = "else";

            theChain.MatchesCategoryOrHttpMethod("something").ShouldBeFalse();
        }

        [Fact]
        public void positive_on_category()
        {
            theChain.UrlCategory.Category = "something";

            theChain.MatchesCategoryOrHttpMethod("something").ShouldBeTrue();
        }

        [Fact]
        public void postive_on_http_verb()
        {
            theChain.UrlCategory.Category = "something";
            theChain.Route.AllowedHttpMethods.Add("POST");
            theChain.Route.AllowedHttpMethods.Add("GET");

            theChain.MatchesCategoryOrHttpMethod("PUT").ShouldBeFalse();
            theChain.MatchesCategoryOrHttpMethod("DELETE").ShouldBeFalse();
            theChain.MatchesCategoryOrHttpMethod("GET").ShouldBeTrue();
            theChain.MatchesCategoryOrHttpMethod("POST").ShouldBeTrue();
            theChain.MatchesCategoryOrHttpMethod("something").ShouldBeTrue();
        }
    }

    
    public class BehaviorChain_determination_of_the_input_type
    {
        [Fact]
        public void uses_the_first_may_have_input_with_non_null()
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(new FakeInputNode(typeof (string)));

            chain.InputType().ShouldBe(typeof (string));

            chain.Prepend(new FakeInputNode(null));
            chain.InputType().ShouldBe(typeof (string));

            chain.Prepend(new FakeInputNode(typeof (int)));
            chain.InputType().ShouldBe(typeof (int));
        }
    }


    
    public class BehaviorChain_determination_of_the_resource_type
    {
        public BehaviorChain_determination_of_the_resource_type()
        {
            theChain = new BehaviorChain();

            strings = new FakeOutputNode(typeof (string));
            none = new FakeOutputNode(null);
            ints = new FakeOutputNode(typeof (int));
        }


        private BehaviorChain theChain;
        private FakeOutputNode strings;
        private FakeOutputNode none;
        private FakeOutputNode ints;

        [Fact]
        public void gets_the_last_resource_type()
        {
            theChain.AddToEnd(none);
            theChain.AddToEnd(strings);
            theChain.AddToEnd(ints);

            theChain.ResourceType().ShouldBe(typeof (int));
        }

        [Fact]
        public void gets_the_last_resource_type_2()
        {
            theChain.AddToEnd(strings);
            theChain.AddToEnd(ints);
            theChain.AddToEnd(none);

            theChain.ResourceType().ShouldBe(typeof (int));
        }

        [Fact]
        public void has_resource_type()
        {
            theChain.HasResourceType().ShouldBeFalse();

            // Void does not count
            theChain.AddToEnd(ActionCall.For<FakeActions>(x => x.Go(null)));
            theChain.HasResourceType().ShouldBeFalse();

            theChain.AddToEnd(none);
            theChain.HasResourceType().ShouldBeFalse();

            theChain.AddToEnd(ints);
            theChain.HasResourceType().ShouldBeTrue();
        }

        [Fact]
        public void in_the_abscence_of_any_criteria_resource_type_is_null()
        {
            theChain.ResourceType().ShouldBeNull();
        }


        [Fact]
        public void override_the_resource_type()
        {
            theChain.ResourceType(typeof (DateTime));

            theChain.ResourceType().ShouldBe(typeof (DateTime));

            theChain.AddToEnd(strings);
            theChain.AddToEnd(ints);
            theChain.AddToEnd(none);

            theChain.ResourceType().ShouldBe(typeof (DateTime));
        }
    }

    public class FakeActions
    {
        public void Go(InputModel model)
        {
        }
    }


    public class FakeInputNode : BehaviorNode, IMayHaveInputType
    {
        private readonly Type _inputType;

        public FakeInputNode(Type inputType)
        {
            _inputType = inputType;
        }

        public override BehaviorCategory Category
        {
            get { throw new NotImplementedException(); }
        }

        public Type InputType()
        {
            return _inputType;
        }

        protected override IConfiguredInstance buildInstance()
        {
            throw new NotImplementedException();
        }
    }

    public class FakeOutputNode : BehaviorNode, IMayHaveResourceType
    {
        private readonly Type _resourceType;

        public FakeOutputNode(Type resourceType)
        {
            _resourceType = resourceType;
        }

        public override BehaviorCategory Category
        {
            get { throw new NotImplementedException(); }
        }

        public Type ResourceType()
        {
            return _resourceType;
        }

        protected override IConfiguredInstance buildInstance()
        {
            throw new NotImplementedException();
        }
    }

    
    public class BehaviorChain_tagging_behavior_Tester
    {
        [Fact]
        public void is_tagged()
        {
            var chain = new BehaviorChain();
            chain.Tags.Add("foo");

            chain.IsTagged("foo").ShouldBeTrue();
            chain.IsTagged("Foo").ShouldBeTrue();
            chain.IsTagged("bar").ShouldBeFalse();
        }
    }
}