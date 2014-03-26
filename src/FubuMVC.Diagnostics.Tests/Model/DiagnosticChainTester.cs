using System.Linq;
using System.Reflection;
using FubuCore.Descriptions;
using FubuMVC.Core.Behaviors.Chrome;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Chrome;
using FubuMVC.Diagnostics.Model;
using FubuMVC.Diagnostics.Requests;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Model
{
    [TestFixture]
    public class DiagnosticChainTester
    {
        private DiagnosticGroup theGroup = new DiagnosticGroup(Assembly.GetExecutingAssembly(), new ActionCall[0]);

        [Test]
        public void is_index_positive()
        {
            var chain = DiagnosticChain.For<FakeFubuDiagnostics>(theGroup, x => x.Index());
            chain.IsIndex.ShouldBeTrue();
        }

        [Test]
        public void is_index_negative()
        {
            DiagnosticChain.For<FakeFubuDiagnostics>(theGroup, x => x.get_link())
                .IsIndex.ShouldBeFalse();
        }

        [Test]
        public void is_link_on_get_with_no_inputs()
        {
            DiagnosticChain.For<FakeFubuDiagnostics>(theGroup, x => x.get_link()).IsLink().ShouldBeTrue();
            DiagnosticChain.For<FakeFubuDiagnostics>(theGroup, x => x.get_simple()).IsLink().ShouldBeTrue();
            DiagnosticChain.For<FakeFubuDiagnostics>(theGroup, x => x.get_else()).IsLink().ShouldBeTrue();
        }

        [Test]
        public void is_detail_positive()
        {
            DiagnosticChain.For<RequestVisualizationFubuDiagnostics>(theGroup, x => x.get_request_details_Id(null)).IsDetailsPage().ShouldBeTrue();
        }

        [Test]
        public void is_link_on_get_with_inputs()
        {
            DiagnosticChain.For<FakeFubuDiagnostics>(theGroup, x => x.get_query_Name(null)).IsLink().ShouldBeFalse();
        }

        [Test]
        public void is_link_on_posts_should_be_false()
        {
            DiagnosticChain.For<FakeFubuDiagnostics>(theGroup, x => x.post_ask()).IsLink().ShouldBeFalse();
        }

        [Test]
        public void build_the_route()
        {
            DiagnosticChain.For<FakeFubuDiagnostics>(theGroup, x => x.get_link())
                .GetRoutePattern().ShouldEqual("_fubu/fake/link");
        }

        [Test]
        public void chain_includes_the_call()
        {
            DiagnosticChain.For<FakeFubuDiagnostics>(theGroup, x => x.get_link())
                .Last()
                .ShouldBeOfType<ActionCall>()
                .Description
                .ShouldEqual("FakeFubuDiagnostics.get_link() : String");
        }

        [Test]
        public void has_the_chrome_node()
        {
            DiagnosticChain.For<FakeFubuDiagnostics>(theGroup, x => x.get_link())
                .First()
                .ShouldBeOfType<ChromeNode>()
                .ContentType
                .ShouldEqual(typeof (DashboardChrome));
        }

        [Test]
        public void url_for_index_method()
        {
            var chain = DiagnosticChain.For<FakeFubuDiagnostics>(theGroup, x => x.Index());

            chain.GetRoutePattern().ShouldEqual("_fubu/fake");
            chain.IsLink().ShouldBeTrue();
        }

        [Test]
        public void title_for_index_is_same_as_group()
        {
            var chain = DiagnosticChain.For<FakeFubuDiagnostics>(theGroup, x => x.Index());
            chain.Title.ShouldEqual(theGroup.Title);
            chain.Description.ShouldEqual(theGroup.Description);
        }

        [Test]
        public void title_for_non_index_with_no_attributes()
        {
            var chain = DiagnosticChain.For<FakeFubuDiagnostics>(theGroup, x => x.get_else());
            chain.Title.ShouldEqual("Else");
            chain.Description.ShouldBeEmpty();
        }

        [Test]
        public void title_for_non_index_with_attribute()
        {
            var chain = DiagnosticChain.For<FakeFubuDiagnostics>(theGroup, x => x.get_simple());
            chain.Title.ShouldEqual("Simple Title");
            chain.Description.ShouldBeEmpty();
        }

        [Test]
        public void title_for_non_index_with_colon_in_the_mix()
        {
            var chain = DiagnosticChain.For<FakeFubuDiagnostics>(theGroup, x => x.get_link());
            chain.Title.ShouldEqual("A Title");
            chain.Description.ShouldEqual("A really good display of some important data");
        }

    }

    public class FakeQuery
    {
        public string Name { get; set; }
    }
}