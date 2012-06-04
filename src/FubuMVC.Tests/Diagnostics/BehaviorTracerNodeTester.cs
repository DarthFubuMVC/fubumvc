using System;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using FubuTestingSupport;
using FubuCore;

namespace FubuMVC.Tests.Diagnostics
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
            theNode.Category.ShouldEqual(BehaviorCategory.Instrumentation);
        }

        [Test]
        public void build_the_object_def()
        {
            var objectDef = theNode.As<IContainerModel>().ToObjectDef(DiagnosticLevel.None);
            objectDef.Type.ShouldEqual(typeof (BehaviorTracer));
            var correlation = objectDef.FindDependencyValueFor<BehaviorCorrelation>();

            correlation.BehaviorId.ShouldEqual(inner.UniqueId);
            correlation.ChainId.ShouldEqual(chain.UniqueId);
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