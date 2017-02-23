using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Registration.Nodes;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Diagnostics.Runtime
{
    
    public class BehaviorTracerNodeTester
    {
        private Wrapper inner;
        private BehaviorChain chain;
        private BehaviorTracerNode theNode;

        public BehaviorTracerNodeTester()
        {
            inner = Wrapper.For<SimpleBehavior>();
            chain = new BehaviorChain();

            chain.AddToEnd(inner);

            theNode = new BehaviorTracerNode(inner);
        }


        [Fact]
        public void should_put_itself_before_the_inner_node()
        {
            theNode.Next.ShouldBeTheSameAs(inner);
        }

        [Fact]
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