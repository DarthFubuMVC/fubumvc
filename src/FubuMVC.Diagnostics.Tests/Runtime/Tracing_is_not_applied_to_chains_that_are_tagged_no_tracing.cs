using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Runtime
{
    [TestFixture]
    public class Tracing_is_not_applied_to_chains_that_are_tagged_no_tracing
    {
        [Test]
        public void not_applied_when_no_tracing_tag_is_on_the_chain()
        {
            using (var runtime = FubuApplication.DefaultPolicies().StructureMap().Bootstrap())
            {
                var graph = runtime.Factory.Get<BehaviorGraph>();

                graph.BehaviorFor<NotTracedEndpoint>(x => x.get_nothing()).OfType<DiagnosticNode>().Any().ShouldBeFalse();
                graph.BehaviorFor<NotTracedEndpoint>(x => x.get_nothing()).OfType<BehaviorTracerNode>().Any().ShouldBeFalse();

                graph.BehaviorFor<NotTracedEndpoint>(x => x.get_something()).OfType<DiagnosticNode>().Any().ShouldBeFalse();
                graph.BehaviorFor<NotTracedEndpoint>(x => x.get_something()).OfType<BehaviorTracerNode>().Any().ShouldBeFalse();

                graph.BehaviorFor<SomeTracingEndpoint>(x => x.get_tracing_no()).OfType<DiagnosticNode>().Any().ShouldBeFalse();
                graph.BehaviorFor<SomeTracingEndpoint>(x => x.get_tracing_no()).OfType<BehaviorTracerNode>().Any().ShouldBeFalse();

                graph.BehaviorFor<SomeTracingEndpoint>(x => x.get_tracing_yes()).OfType<DiagnosticNode>().Any().ShouldBeTrue();
                graph.BehaviorFor<SomeTracingEndpoint>(x => x.get_tracing_yes()).OfType<BehaviorTracerNode>().Any().ShouldBeTrue();
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