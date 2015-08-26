using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Diagnostics.Runtime
{
    [TestFixture]
    public class BehaviorTracerNodeTester
    {
        private Wrapper inner;
        private BehaviorChain chain;
        private BehaviorTracerNode theNode;

        [SetUp]
        public void SetUp()
        {
            inner = Wrapper.For<SimpleBehavior>();
            chain = new BehaviorChain();

            chain.AddToEnd(inner);

            theNode = new BehaviorTracerNode(inner);
        }

        [Test]
        public void should_put_itself_before_the_inner_node()
        {
            theNode.Next.ShouldBeTheSameAs(inner);
        }

        [Test]
        public void category_has_to_be_instrumentation()
        {
            theNode.Category.ShouldBe(BehaviorCategory.Instrumentation);
        }


        public class SimpleBehavior : IActionBehavior
        {
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
}