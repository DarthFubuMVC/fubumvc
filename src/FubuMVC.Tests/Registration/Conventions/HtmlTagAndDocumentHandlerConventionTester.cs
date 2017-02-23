using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.Ajax;
using Shouldly;
using HtmlTags;
using Xunit;
using OutputNode = FubuMVC.Core.Resources.Conneg.OutputNode;

namespace FubuMVC.Tests.Registration.Conventions
{
    
    public class HtmlTagAndDocumentHandlerConventionTester
    {
        private BehaviorGraph graph = BehaviorGraph.BuildFrom(x =>
        {
            x.Actions.IncludeType<TagController>();
        });

        [Fact]
        public void action_that_returns_HtmlDocument_should_output_to_html()
        {
            var outputNode = graph.ChainFor<TagController>(x => x.BuildDoc()).Outputs.First().ShouldBeOfType<OutputNode>();
            outputNode
                .ResourceType.ShouldBe(typeof(HtmlDocument));

            outputNode.Writes(MimeType.Html).ShouldBeTrue();
        }

        [Fact]
        public void action_that_returns_HtmlTag_should_output_to_html()
        {
            var outputNode =
                graph.ChainFor<TagController>(x => x.BuildTag()).Outputs.First().ShouldBeOfType<OutputNode>();
            outputNode.Writes(MimeType.Html).ShouldBeTrue();
            outputNode.ResourceType.ShouldBe(typeof(HtmlTag));
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