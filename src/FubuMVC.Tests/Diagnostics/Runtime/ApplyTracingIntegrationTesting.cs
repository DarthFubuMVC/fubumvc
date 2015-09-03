using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Diagnostics.Runtime
{
    [TestFixture]
    public class ApplyTracingIntegrationTesting
    {
        private RoutedChain chain1;
        private BehaviorChain chain2;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Configure(graph =>
            {
                chain1 = new RoutedChain("something");
                chain1.AddToEnd(Wrapper.For<SimpleBehavior>());
                chain1.AddToEnd(Wrapper.For<DifferentBehavior>());
                graph.AddChain(chain1);

                chain2 = new BehaviorChain();
                chain2.IsPartialOnly = true;
                chain2.AddToEnd(Wrapper.For<SimpleBehavior>());
                chain2.AddToEnd(Wrapper.For<DifferentBehavior>());
                graph.AddChain(chain2);
            });

            registry.Features.Diagnostics.Enable(TraceLevel.Verbose);


            BehaviorGraph.BuildFrom(registry);
        }

        [Test]
        public void do_nothing_if_tracing_is_off()
        {
            var registry = new FubuRegistry();
            registry.Features.Diagnostics.Enable(TraceLevel.None);
            registry.Configure(graph =>
            {
                chain1 = new RoutedChain("something");
                chain1.AddToEnd(Wrapper.For<SimpleBehavior>());
                chain1.AddToEnd(Wrapper.For<DifferentBehavior>());
                graph.AddChain(chain1);

                chain2 = new BehaviorChain();
                chain2.IsPartialOnly = true;
                chain2.AddToEnd(Wrapper.For<SimpleBehavior>());
                chain2.AddToEnd(Wrapper.For<DifferentBehavior>());
                graph.AddChain(chain2);
            });


            var notTracedGraph = BehaviorGraph.BuildFrom(registry);
            notTracedGraph.Chains.SelectMany(x => x).Any(x => x is BehaviorTracerNode).ShouldBeFalse();
        }

        [Test]
        public void full_chain()
        {
            chain1.ElementAt(0).ShouldBeOfType<BehaviorTracerNode>();
            chain1.ElementAt(1).ShouldBeOfType<Wrapper>();
            chain1.ElementAt(2).ShouldBeOfType<BehaviorTracerNode>();
            chain1.ElementAt(3).ShouldBeOfType<Wrapper>();
        }

        [Test]
        public void partial_chain_should_not_have_diagnostic_behavior()
        {
            chain2.ElementAt(0).ShouldBeOfType<BehaviorTracerNode>();
            chain2.ElementAt(1).ShouldBeOfType<Wrapper>();
            chain2.ElementAt(2).ShouldBeOfType<BehaviorTracerNode>();
            chain2.ElementAt(3).ShouldBeOfType<Wrapper>();
        }


        public class SimpleBehavior : IActionBehavior
        {
            public void Invoke()
            {
            }

            public void InvokePartial()
            {
            }
        }

        public class DifferentBehavior : SimpleBehavior
        {
        }
    }
}