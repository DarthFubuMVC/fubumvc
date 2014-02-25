using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.Ajax;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using OutputNode = FubuMVC.Core.Resources.Conneg.OutputNode;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class HtmlTagAndDocumentHandlerConventionTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<TagController>();
            });
        }

        #endregion

        private BehaviorGraph graph;

        [Test]
        public void action_that_returns_HtmlDocument_should_output_to_html()
        {
            var outputNode = graph.BehaviorFor<TagController>(x => x.BuildDoc()).Outputs.First().ShouldBeOfType<OutputNode>();
            outputNode
                .ResourceType.ShouldEqual(typeof(HtmlDocument));

            outputNode.Writes(MimeType.Html).ShouldBeTrue();
        }

        [Test]
        public void action_that_returns_HtmlTag_should_output_to_html()
        {
            var outputNode =
                graph.BehaviorFor<TagController>(x => x.BuildTag()).Outputs.First().ShouldBeOfType<OutputNode>();
            outputNode.Writes(MimeType.Html).ShouldBeTrue();
            outputNode.ResourceType.ShouldEqual(typeof(HtmlTag));
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