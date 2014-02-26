using System;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Formatters;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class when_querying_a_behavior_node_for_has_after
    {
        private Wrapper node1;
        private Wrapper node2;
        private Wrapper node3;

        [SetUp]
        public void SetUp()
        {
            node1 = new Wrapper(typeof(FakeJsonBehavior));
            node2 = new Wrapper(typeof(FakeJsonBehavior));
            node3 = new Wrapper(typeof(FakeJsonBehavior));
            node1.AddToEnd(node2);
            node1.AddToEnd(node3);
        }


        [Test]
        public void find_the_follower_positive()
        {
            var found = MockRepository.GenerateMock<Action<BehaviorNode>>();
            var missing = MockRepository.GenerateMock<Action>();
            var search = new BehaviorSearch(n => ReferenceEquals(n, node3)){
                OnFound = found,
                OnMissing = missing
            };

            node1.ForFollowingBehavior(search);

            found.AssertWasCalled(x => x.Invoke(node3));
            missing.AssertWasNotCalled(x => x.Invoke());
        }


        [Test]
        public void find_the_follower_positive_2()
        {
            var found = MockRepository.GenerateMock<Action<BehaviorNode>>();
            var missing = MockRepository.GenerateMock<Action>();
            var search = new BehaviorSearch(n => ReferenceEquals(n, node3))
            {
                OnFound = found,
                OnMissing = missing
            };

            node2.ForFollowingBehavior(search);

            found.AssertWasCalled(x => x.Invoke(node3));
            missing.AssertWasNotCalled(x => x.Invoke());
        }


        [Test]
        public void find_the_follower_negative()
        {
            var found = MockRepository.GenerateMock<Action<BehaviorNode>>();
            var missing = MockRepository.GenerateMock<Action>();
            var search = new BehaviorSearch(n => false)
            {
                OnFound = found,
                OnMissing = missing
            };

            node1.ForFollowingBehavior(search);

            found.AssertWasNotCalled(x => x.Invoke(null), x => x.IgnoreArguments());
            missing.AssertWasCalled(x => x.Invoke());
        }

    }

    public class FakeJsonBehavior : IActionBehavior
    {
        public FakeJsonBehavior(IFormatter writer, IFubuRequest request, IRequestData data)
        {
            Writer = writer;
            Request = request;
            Data = data;
        }

        public IFormatter Writer { get; set; }
        public IFubuRequest Request { get; set; }
        public IRequestData Data { get; set; }
        public void Invoke()
        {
            throw new NotImplementedException();
        }

        public void InvokePartial()
        {
            throw new NotImplementedException();
        }
    }
}