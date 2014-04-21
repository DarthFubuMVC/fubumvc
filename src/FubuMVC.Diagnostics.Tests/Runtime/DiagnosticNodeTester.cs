using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Registration.Nodes;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Runtime
{
    [TestFixture]
    public class DiagnosticNodeTester
    {
        private Wrapper inner;
        private BehaviorChain chain;
        private DiagnosticNode theNode;

        [SetUp]
        public void SetUp()
        {
            inner = Wrapper.For<SimpleBehavior>();
            chain = new BehaviorChain();

            chain.AddToEnd(inner);

            theNode = new DiagnosticNode(chain);
        }

        [Test]
        public void puts_itself_at_the_start_of_the_chain()
        {
            chain.Top.ShouldBeTheSameAs(theNode);
        }

        [Test]
        public void category_has_to_be_instrumentation()
        {
            theNode.Category.ShouldEqual(BehaviorCategory.Instrumentation);
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