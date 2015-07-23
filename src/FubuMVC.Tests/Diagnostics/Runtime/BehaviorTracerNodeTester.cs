using System;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Diagnostics.Runtime.Tracing;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap.Pipeline;

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

        [Test]
        public void build_the_object_def()
        {
            var objectDef = theNode.As<IContainerModel>().ToInstance().As<IConfiguredInstance>();
            objectDef.PluggedType.ShouldBe(typeof (BehaviorTracer));
            var correlation = objectDef.FindDependencyValueFor<BehaviorCorrelation>();

            correlation.Node.ShouldBe(inner);
            correlation.ChainId.ShouldBe(chain.UniqueId);
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