using System.Diagnostics;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class DiagnosticsRegistryTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            graph = new DiagnosticsRegistry().BuildGraph();
        }

        #endregion

        private BehaviorGraph graph;

        [Test]
        public void actions_url()
        {
            graph.BehaviorFor<BehaviorGraphWriter>(x => x.Actions()).Route.Pattern.ShouldEqual("_fubu/actions");
        }

        [Test]
        public void index_action_url()
        {
            graph.BehaviorFor<BehaviorGraphWriter>(x => x.Index()).Route.Pattern.ShouldEqual("_fubu");
        }

        [Test]
        public void index_action_writes_out_to_html_document()
        {
            graph.BehaviorFor<BehaviorGraphWriter>(x => x.Index()).Outputs.First().ShouldBeOfType
                <RenderHtmlDocumentNode>();
        }


        [Test]
        public void smoke_test()
        {
            Debug.WriteLine(new BehaviorGraphWriter(graph).PrintRoutes());
        }
    }

    public class WrappingBehavior : BasicBehavior
    {
        public WrappingBehavior(PartialBehavior partialBehavior)
            : base(partialBehavior)
        {
        }

        protected override DoNext performInvoke()
        {
            return DoNext.Continue;
        }
    }

    [TestFixture]
    public class include_diagnostics_integration_tester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            graph = new FubuRegistry(x =>
            {
                x.IncludeDiagnostics(true);
                x.Applies.ToThisAssembly();
                x.Actions.IncludeTypes(o => o.Name.Contains("Controller"));
            }).BuildGraph();
        }

        #endregion

        private BehaviorGraph graph;


        [Test]
        public void actions_url()
        {
            graph.BehaviorFor<BehaviorGraphWriter>(x => x.Actions()).Route.Pattern.ShouldEqual("_fubu/actions");
        }

        [Test]
        public void each_behavior_chain_should_start_with_diagnostic_behavior()
        {
            graph.BehaviorChainCount.ShouldBeGreaterThan(0);
            graph.Behaviors.Each(
                b => b.Top.ShouldBeOfType<Wrapper>().BehaviorType.ShouldEqual(typeof (DiagnosticBehavior)));
        }

        [Test]
        public void has_the_debugging_services_registred()
        {
            graph.Services.DefaultServiceFor<IOutputWriter>().Type.ShouldEqual(typeof (DebuggingOutputWriter));
        }

        [Test]
        public void index_action_url()
        {
            graph.BehaviorFor<BehaviorGraphWriter>(x => x.Index()).Route.Pattern.ShouldEqual("_fubu");
        }

        [Test]
        public void index_action_writes_out_to_html_document()
        {
            graph.BehaviorFor<BehaviorGraphWriter>(x => x.Index()).Outputs.First().ShouldBeOfType
                <RenderHtmlDocumentNode>();
        }
    }

    [TestFixture]
    public class the_index_action_on_the_behavior_graph_writer : InteractionContext<BehaviorGraphWriter>
    {
        private HtmlDocument _output;

        protected override void beforeEach()
        {
            _output = ClassUnderTest.Index();
        }

        [Test]
        public void should_include_links_for_the_public_instance_methods_with_description_attributes()
        {
            HtmlTag child = _output.Current.Children[0];
            child.ToString().IndexOf("Routes").ShouldNotEqual(-1);
            child.ToString().IndexOf("Actions").ShouldNotEqual(-1);
        }
    }
}