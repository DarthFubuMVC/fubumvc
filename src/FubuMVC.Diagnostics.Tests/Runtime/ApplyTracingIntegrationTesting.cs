using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Diagnostics.Runtime.Tracing;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Runtime
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

            registry.AlterSettings<DiagnosticsSettings>(x => x.TraceLevel = TraceLevel.Verbose);


            BehaviorGraph.BuildFrom(registry);
        }

        [Test]
        public void do_nothing_if_tracing_is_off()
        {
            var registry = new FubuRegistry();
            registry.AlterSettings<DiagnosticsSettings>(x => x.TraceLevel = TraceLevel.None);
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
            notTracedGraph.Behaviors.SelectMany(x => x).Any(x => x is DiagnosticBehavior).ShouldBeFalse();
            notTracedGraph.Behaviors.SelectMany(x => x).Any(x => x is BehaviorTracer).ShouldBeFalse();
        }

        [Test]
        public void full_chain()
        {
            chain1.First().ShouldBeOfType<DiagnosticNode>();
            chain1.ElementAt(1).ShouldBeOfType<BehaviorTracerNode>();
            chain1.ElementAt(2).ShouldBeOfType<Wrapper>();
            chain1.ElementAt(3).ShouldBeOfType<BehaviorTracerNode>();
            chain1.ElementAt(4).ShouldBeOfType<Wrapper>();
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

        public class DifferentBehavior : SimpleBehavior { }
    }
}