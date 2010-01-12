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

            chain.Append(wrapper);

            chain.Top.ShouldBeTheSameAs(wrapper);
        }

        [Test]
        public void calls_finds_all_calls_underneath_the_chain()
        {
            var chain = new BehaviorChain();
            ActionCall call = ActionCall.For<TestController>(x => x.AnotherAction(null));
            ActionCall call2 = ActionCall.For<TestController>(x => x.AnotherAction(null));
            chain.Append(call);
            chain.Append(call2);

            chain.Calls.Count().ShouldEqual(2);
            chain.Calls.Contains(call).ShouldBeTrue();
            chain.Calls.Contains(call2).ShouldBeTrue();
        }

        [Test]
        public void has_input()
        {
            var chain = new BehaviorChain();
            chain.HasInput().ShouldBeFalse();

            chain.Append(ActionCall.For<ControllerTarget>(x => x.ZeroInOneOut()));
            chain.HasInput().ShouldBeFalse();

            chain = new BehaviorChain();
            chain.Append(ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null)));
            chain.HasInput().ShouldBeTrue();
        }

        [Test]
        public void insert_before_on_a_node()
        {
            var chain = new BehaviorChain();
            var wrapper = new Wrapper(typeof (ObjectDefInstanceTester.FakeJsonBehavior));

            chain.Append(wrapper);

            var wrapper2 = new Wrapper(typeof (ObjectDefInstanceTester.FakeJsonBehavior));

            wrapper.InsertDirectlyBefore(wrapper2);

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
            chain.Append(call);

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
        public void setting_next_on_a_node_also_sets_the_previous()
        {
            var chain = new BehaviorChain();
            var wrapper = new Wrapper(typeof (ObjectDefInstanceTester.FakeJsonBehavior));

            chain.Append(wrapper);

            wrapper.Previous.ShouldBeTheSameAs(chain);

            var wrapper2 = new Wrapper(typeof (ObjectDefInstanceTester.FakeJsonBehavior));

            wrapper.Append(wrapper2);

            wrapper2.Previous.ShouldBeTheSameAs(wrapper);
            wrapper.Previous.ShouldBeTheSameAs(chain);
        }


        [Test]
        public void setting_next_on_a_node_also_sets_the_previous_2()
        {
            var chain = new BehaviorChain();
            var wrapper = new Wrapper(typeof (ObjectDefInstanceTester.FakeJsonBehavior));

            chain.Append(wrapper);

            wrapper.Previous.ShouldBeTheSameAs(chain);

            var wrapper2 = new Wrapper(typeof (ObjectDefInstanceTester.FakeJsonBehavior));

            chain.Append(wrapper2);

            wrapper2.Previous.ShouldBeTheSameAs(wrapper);
            wrapper.Previous.ShouldBeTheSameAs(chain);
        }

        [Test]
        public void the_unique_id_of_the_behavior_chain_matches_the_object_def_name()
        {
            var chain = new BehaviorChain();
            ActionCall call = ActionCall.For<TestController>(x => x.AnotherAction(null));
            chain.Append(call);

            chain.UniqueId.ToString().ShouldEqual(chain.ToObjectDef().Name);
        }
    }
}