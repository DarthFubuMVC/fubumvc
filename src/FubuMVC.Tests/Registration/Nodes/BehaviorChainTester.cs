using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Security;
using FubuMVC.FakeControllers;
using FubuMVC.StructureMap;
using FubuMVC.Tests.StructureMapIoC;
using FubuMVC.Tests.View;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using StructureMap;

namespace FubuMVC.Tests.Registration.Nodes
{
    [TestFixture]
    public class BehaviorChainTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void append_with_no_behaviors()
        {
            var chain = new BehaviorChain();
            var wrapper = new Wrapper(typeof (ObjectDefInstanceTester.FakeJsonBehavior));

            chain.AddToEnd(wrapper);

            chain.Top.ShouldBeTheSameAs(wrapper);
        }

        [Test]
        public void calls_finds_all_calls_underneath_the_chain()
        {
            var chain = new BehaviorChain();
            ActionCall call = ActionCall.For<TestController>(x => x.AnotherAction(null));
            ActionCall call2 = ActionCall.For<TestController>(x => x.AnotherAction(null));
            chain.AddToEnd(call);
            chain.AddToEnd(call2);

            chain.Calls.Count().ShouldEqual(2);
            chain.Calls.Contains(call).ShouldBeTrue();
            chain.Calls.Contains(call2).ShouldBeTrue();
        }

        [Test]
        public void has_input()
        {
            var chain = new BehaviorChain();
            chain.HasInput().ShouldBeFalse();

            chain.AddToEnd(ActionCall.For<ControllerTarget>(x => x.ZeroInOneOut()));
            chain.HasInput().ShouldBeFalse();

            chain = new BehaviorChain();
            chain.AddToEnd(ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null)));
            chain.HasInput().ShouldBeTrue();
        }

        [Test]
        public void insert_before_on_a_node()
        {
            var chain = new BehaviorChain();
            var wrapper = new Wrapper(typeof (ObjectDefInstanceTester.FakeJsonBehavior));

            chain.AddToEnd(wrapper);

            var wrapper2 = new Wrapper(typeof (ObjectDefInstanceTester.FakeJsonBehavior));

            wrapper.AddBefore(wrapper2);

            chain.Top.ShouldBeTheSameAs(wrapper2);
            wrapper2.Next.ShouldBeTheSameAs(wrapper);

            wrapper2.Previous.ShouldBeTheSameAs(chain);
            wrapper.Previous.ShouldBeTheSameAs(wrapper2);
        }

        [Test]
        public void prepend_with_an_existing_top_behavior()
        {
            var chain = new BehaviorChain();
            ActionCall call = ActionCall.For<TestController>(x => x.AnotherAction(null));
            chain.AddToEnd(call);

            var wrapper = new Wrapper(typeof (ObjectDefInstanceTester.FakeJsonBehavior));
            chain.Prepend(wrapper);

            chain.Top.ShouldBeTheSameAs(wrapper);
            wrapper.Next.ShouldBeTheSameAs(call);
        }

        [Test]
        public void prepend_with_no_behaviors()
        {
            var chain = new BehaviorChain();
            var wrapper = new Wrapper(typeof (ObjectDefInstanceTester.FakeJsonBehavior));

            chain.Prepend(wrapper);

            chain.Top.ShouldBeTheSameAs(wrapper);
        }

        [Test]
        public void appending_a_node_also_sets_the_previous()
        {
            var chain = new BehaviorChain();
            var wrapper = new Wrapper(typeof (ObjectDefInstanceTester.FakeJsonBehavior));

            chain.AddToEnd(wrapper);

            wrapper.Previous.ShouldBeTheSameAs(chain);

            var wrapper2 = new Wrapper(typeof (ObjectDefInstanceTester.FakeJsonBehavior));

            wrapper.AddToEnd(wrapper2);

            wrapper2.Previous.ShouldBeTheSameAs(wrapper);
            wrapper.Previous.ShouldBeTheSameAs(chain);
        }


        [Test]
        public void appending_a_node_also_sets_the_previous_2()
        {
            var chain = new BehaviorChain();
            var wrapper = new Wrapper(typeof (ObjectDefInstanceTester.FakeJsonBehavior));

            chain.AddToEnd(wrapper);

            wrapper.Previous.ShouldBeTheSameAs(chain);

            var wrapper2 = new Wrapper(typeof (ObjectDefInstanceTester.FakeJsonBehavior));

            chain.AddToEnd(wrapper2);

            wrapper2.Previous.ShouldBeTheSameAs(wrapper);
            wrapper.Previous.ShouldBeTheSameAs(chain);
        }

        [Test]
        public void removing_a_node_maintains_the_link_between_its_predecessor_and_successor()
        {
            var node1 = new Wrapper(typeof(ObjectDefInstanceTester.FakeJsonBehavior));
            var node2 = new Wrapper(typeof(ObjectDefInstanceTester.FakeJsonBehavior));
            var node3 = new Wrapper(typeof(ObjectDefInstanceTester.FakeJsonBehavior));
            node1.AddToEnd(node2);
            node1.AddToEnd(node3);
            node2.Remove();

            node2.Previous.ShouldBeNull();
            node2.Next.ShouldBeNull();
            node1.Next.ShouldBeTheSameAs(node3);
            node3.Previous.ShouldBeTheSameAs(node1);
        }

        [Test]
        public void removing_a_node_without_a_predecessor_sets_its_successor_to_the_front()
        {
            var node1 = new Wrapper(typeof(ObjectDefInstanceTester.FakeJsonBehavior));
            var node2 = new Wrapper(typeof(ObjectDefInstanceTester.FakeJsonBehavior));
            node1.AddToEnd(node2);

            node1.Remove();

            node1.Previous.ShouldBeNull();
            node1.Next.ShouldBeNull();
            node2.Previous.ShouldBeNull();
        }

        [Test]
        public void replacing_a_node_should_disconnect_the_node_being_replaced()
        {
            var node1 = new Wrapper(typeof(ObjectDefInstanceTester.FakeJsonBehavior));
            var node2 = new Wrapper(typeof(ObjectDefInstanceTester.FakeJsonBehavior));
            var node3 = new Wrapper(typeof(ObjectDefInstanceTester.FakeJsonBehavior));
            var newNode = new Wrapper(typeof(ObjectDefInstanceTester.FakeJsonBehavior));
            node1.AddToEnd(node2);
            node1.AddToEnd(node3);
            node2.ReplaceWith(newNode);

            node2.Next.ShouldBeNull();
            node2.Previous.ShouldBeNull();
        }


        [Test]
        public void replacing_a_node_should_set_the_new_nodes_predecessor_and_successor()
        {
            var node1 = new Wrapper(typeof(ObjectDefInstanceTester.FakeJsonBehavior));
            var node2 = new Wrapper(typeof(ObjectDefInstanceTester.FakeJsonBehavior));
            var node3 = new Wrapper(typeof(ObjectDefInstanceTester.FakeJsonBehavior));
            var newNode = new Wrapper(typeof(ObjectDefInstanceTester.FakeJsonBehavior));
            node1.AddToEnd(node2);
            node1.AddToEnd(node3);
            node2.ReplaceWith(newNode);

            newNode.Previous.ShouldBeTheSameAs(node1);
            newNode.Next.ShouldBeTheSameAs(node3);
            node1.Next.ShouldBeTheSameAs(newNode);
            node3.Previous.ShouldBeTheSameAs(newNode);
        }

        [Test]
        public void replacing_a_node_without_a_predecessor_should_set_the_new_node_to_the_front()
        {
            var node1 = new Wrapper(typeof(ObjectDefInstanceTester.FakeJsonBehavior));
            var node2 = new Wrapper(typeof(ObjectDefInstanceTester.FakeJsonBehavior));
            var newNode = new Wrapper(typeof(ObjectDefInstanceTester.FakeJsonBehavior));
            node1.AddToEnd(node2);
            node1.ReplaceWith(newNode);

            newNode.Previous.ShouldBeNull();
            newNode.Next.ShouldBeTheSameAs(node2);
        }


        [Test]
        public void the_unique_id_of_the_behavior_chain_matches_the_object_def_name()
        {
            var chain = new BehaviorChain();
            ActionCall call = ActionCall.For<TestController>(x => x.AnotherAction(null));
            chain.AddToEnd(call);

            chain.UniqueId.ToString().ShouldEqual(chain.ToObjectDef().Name);
        }

        [Test]
        public void finding_the_chain_from_a_chain_returns_itself()
        {
            var chain = new BehaviorChain();
            chain.ParentChain().ShouldBeTheSameAs(chain);
        }

        [Test]
        public void find_the_chain_when_the_parent_is_null_should_be_null()
        {
            var node = new Wrapper(typeof (ObjectDefInstanceTester.FakeJsonBehavior));
            node.ParentChain().ShouldBeNull();
        }

        [Test]
        public void find_the_parent_chain_when_the_chain_is_the_immediate_parent()
        {
            var chain = new BehaviorChain();
            var node = new Wrapper(typeof(ObjectDefInstanceTester.FakeJsonBehavior));
            chain.AddToEnd(node);
            node.ParentChain().ShouldBeTheSameAs(chain);
        }

        [Test]
        public void find_the_parent_from_deep_in_behavior_chain()
        {
            var chain = new BehaviorChain();
            var node = new Wrapper(typeof(ObjectDefInstanceTester.FakeJsonBehavior));
            chain.AddToEnd(node);

            var node2 = new Wrapper(typeof(ObjectDefInstanceTester.FakeJsonBehavior));
            node.AddToEnd(node2);

            var node3 = new Wrapper(typeof(ObjectDefInstanceTester.FakeJsonBehavior));
            node2.AddToEnd(node3);

            node3.ParentChain().ShouldBeTheSameAs(chain);
        }

        [Test]
        public void category_returns_chain()
        {
            var chain = new BehaviorChain();
            chain.Category.ShouldEqual(BehaviorCategory.Chain);
        }

        [Test]
        public void returns_input_type_name_if_first_call_is_not_null()
        {
            var chain = new BehaviorChain();
            Type type = typeof(ControllerTarget);
            chain.AddToEnd(new ActionCall(type, type.GetMethod("OneInOneOut")));
            chain.InputTypeName.ShouldEqual("Model1");
        }

        [Test]
        public void input_type_name_return_empty_if_first_call_is_null()
        {
            var chain = new BehaviorChain();
            chain.InputTypeName.ShouldEqual("");
        }

        [Test]
        public void return_description_if_first_call_is_not_null()
        {
            var chain = new BehaviorChain();
            Type type = typeof(ControllerTarget);
            chain.AddToEnd(new ActionCall(type, type.GetMethod("OneInZeroOut")));
            chain.FirstCallDescription.ShouldEqual("ControllerTarget.OneInZeroOut(Model1 input) : void");
        }

        [Test]
        public void first_call_description_return_empty_if_first_call_is_null()
        {
            var chain = new BehaviorChain();
            chain.FirstCallDescription.ShouldEqual("");
        }

        [Test]
        public void get_enum_should_yield_top()
        {
            var firstNode = new BehaviorChain();
            var secondNode = new BehaviorChain();
            var thirdNode = new BehaviorChain();
            firstNode.AddToEnd(secondNode);
            secondNode.AddToEnd(thirdNode);
            var enumerator = firstNode.GetEnumerator();
            enumerator.MoveNext();
            enumerator.Current.ShouldEqual(secondNode);
            enumerator.MoveNext();
            enumerator.Current.ShouldEqual(thirdNode);
            enumerator.MoveNext().ShouldBeFalse();
        }

        [Test]
        public void behavior_node_get_enum_should_yield_next()
        {
            BehaviorNode node = new FakeViewToken();
            var secondNode = new BehaviorChain();
            var thirdNode = new BehaviorChain();
            node.AddAfter(secondNode);
            secondNode.AddAfter(thirdNode);
            var enumerator = node.GetEnumerator();
            enumerator.MoveNext();
            enumerator.Current.ShouldEqual(secondNode);
            enumerator.MoveNext();
            enumerator.Current.ShouldEqual(thirdNode);
            enumerator.MoveNext().ShouldBeFalse();
        }

        [Test]
        public void should_register_an_endpoint_authorizor_if_there_are_any_authorization_rules()
        {
            
            var chain = new BehaviorChain();
            chain.AddToEnd(ActionCall.For<OneController>(x => x.Query(null)));
            chain.Authorization.AddRole("Role 1");
            chain.Prepend(chain.Authorization);

            var container = new Container();
            var facility = new StructureMapContainerFacility(container);

            chain.Register(facility.Register);

            facility.BuildFactory();

            container.GetInstance<IEndPointAuthorizor>(chain.UniqueId.ToString())
                .ShouldNotBeNull().ShouldBeOfType<EndPointAuthorizor>();
        }

        [Test]
        public void should_not_register_an_endpoint_authorizor_if_there_are_no_authorization_roles()
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(ActionCall.For<OneController>(x => x.Query(null)));
            //chain.Authorization.AddRole("Role 1");

            var container = new Container();
            var facility = new StructureMapContainerFacility(container);

            chain.Register(facility.Register);

            facility.BuildFactory();

            Debug.WriteLine(chain.UniqueId);
            Debug.WriteLine(container.WhatDoIHave());

            container.GetInstance<IEndPointAuthorizor>(chain.UniqueId.ToString())
                .ShouldBeOfType<NulloEndPointAuthorizor>();
        }


    }


}