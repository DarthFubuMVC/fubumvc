using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Diagnostics.Runtime
{
    [TestFixture]
    public class Tracing_is_not_applied_to_chains_that_are_tagged_no_tracing
    {
        [Test]
        public void not_applied_when_no_tracing_tag_is_on_the_chain()
        {
            using (var runtime = FubuRuntime.Basic(_ => _.Mode = "development"))
            {
                var graph = runtime.Factory.Get<BehaviorGraph>();

                graph.ChainFor<NotTracedEndpoint>(x => x.get_nothing())
                    .OfType<DiagnosticNode>()
                    .Any()
                    .ShouldBeFalse();
                graph.ChainFor<NotTracedEndpoint>(x => x.get_nothing())
                    .OfType<BehaviorTracerNode>()
                    .Any()
                    .ShouldBeFalse();

                graph.ChainFor<NotTracedEndpoint>(x => x.get_something())
                    .OfType<DiagnosticNode>()
                    .Any()
                    .ShouldBeFalse();
                graph.ChainFor<NotTracedEndpoint>(x => x.get_something())
                    .OfType<BehaviorTracerNode>()
                    .Any()
                    .ShouldBeFalse();

                graph.ChainFor<SomeTracingEndpoint>(x => x.get_tracing_no())
                    .OfType<DiagnosticNode>()
                    .Any()
                    .ShouldBeFalse();
                graph.ChainFor<SomeTracingEndpoint>(x => x.get_tracing_no())
                    .OfType<BehaviorTracerNode>()
                    .Any()
                    .ShouldBeFalse();

                graph.ChainFor<SomeTracingEndpoint>(x => x.get_tracing_yes())
                    .OfType<DiagnosticNode>()
                    .Any()
                    .ShouldBeTrue();
                graph.ChainFor<SomeTracingEndpoint>(x => x.get_tracing_yes())
                    .OfType<BehaviorTracerNode>()
                    .Any()
                    .ShouldBeTrue();
            }
        }
    }

    [Tag(BehaviorChain.NoTracing)]
    public class NotTracedEndpoint
    {
        public string get_nothing()
        {
            return "nothing";
        }

        public string get_something()
        {
            return "something";
        }
    }

    public class SomeTracingEndpoint
    {
        public string get_tracing_yes()
        {
            return "yes";
        }

        [Tag(BehaviorChain.NoTracing)]
        public string get_tracing_no()
        {
            return "no";
        }
    }
}