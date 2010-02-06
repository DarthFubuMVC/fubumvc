using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Tests.StructureMapIoC;
using NUnit.Framework;

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
    }
}