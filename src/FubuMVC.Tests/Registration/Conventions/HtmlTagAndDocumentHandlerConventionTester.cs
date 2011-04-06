using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class HtmlTagAndDocumentHandlerConventionTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            graph = new FubuRegistry(x => { x.Actions.IncludeType<TagController>(); }).BuildGraph();
        }

        #endregion

        private BehaviorGraph graph;

        [Test]
        public void action_that_returns_HtmlDocument_should_output_to_html()
        {
            graph.BehaviorFor<TagController>(x => x.BuildDoc()).Outputs.First().ShouldBeOfType<RenderHtmlDocumentNode>();
        }

        [Test]
        public void action_that_returns_HtmlTag_should_output_to_html()
        {
            graph.BehaviorFor<TagController>(x => x.BuildTag()).Outputs.First().ShouldBeOfType<RenderHtmlTagNode>();
        }
    }

    public class TagController
    {
        public string GoSomewhere()
        {
            return null;
        }

        public HtmlTag BuildTag()
        {
            return null;
        }

        public HtmlDocument BuildDoc()
        {
            return null;
        }
    }
}