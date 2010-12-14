using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using HtmlTags;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class DiagnosticsRegistryTester
    {
        [SetUp]
        public void SetUp()
        {
            graph = new DiagnosticsRegistry().BuildGraph();
            urls = MockRepository.GenerateMock<IUrlRegistry>();

            graph.Behaviors.Any().ShouldBeTrue();
            graph.Actions().Each(x => Debug.WriteLine(x.Description));
        }

        private BehaviorGraph graph;
        private IUrlRegistry urls;

        [Test]
        public void actions_url()
        {
            graph.BehaviorFor<BehaviorGraphWriter>(x => x.Actions()).Route.Pattern.ShouldEqual(DiagnosticUrlPolicy.DIAGNOSTICS_URL_ROOT + "/actions");
        }

        [Test]
        public void index_action_url()
        {
            graph.BehaviorFor<BehaviorGraphWriter>(x => x.Index()).Route.Pattern.ShouldEqual(DiagnosticUrlPolicy.DIAGNOSTICS_URL_ROOT);
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
            Debug.WriteLine(new BehaviorGraphWriter(graph, urls, null).PrintRoutes());
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
                x.Actions.IncludeTypes(o => o.Name.Contains("Controller"));
            }).BuildGraph();
        }

        #endregion

        private BehaviorGraph graph;


        [Test]
        public void actions_url()
        {
            graph.BehaviorFor<BehaviorGraphWriter>(x => x.Actions()).Route.Pattern.ShouldEqual(DiagnosticUrlPolicy.DIAGNOSTICS_URL_ROOT + "/actions");
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
            graph.BehaviorFor<BehaviorGraphWriter>(x => x.Index()).Route.Pattern.ShouldEqual(DiagnosticUrlPolicy.DIAGNOSTICS_URL_ROOT);
        }

        [Test]
        public void index_action_writes_out_to_html_document()
        {
            graph.BehaviorFor<BehaviorGraphWriter>(x => x.Index()).Outputs.First().ShouldBeOfType
                <RenderHtmlDocumentNode>();
        }
    }

}