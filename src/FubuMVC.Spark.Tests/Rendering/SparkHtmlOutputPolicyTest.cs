using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using System.Linq;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class SparkHtmlOutputPolicyTest 
    {
        private BehaviorGraph _graph;

        [SetUp]
        public void SetUp()
        {
            var outputPolicy = new SparkHtmlOutputPolicy();
            
            _graph = new FubuRegistry(r => r.Actions.IncludeType<HtmlController>()).BuildGraph();
            _graph.Behaviors.ShouldHaveCount(2);

            outputPolicy.Configure(_graph);
        }

        [Test]
        public void should_replace_output_for_actions_returning_html_tag_with_spark_document_output()
        {
            _graph.BehaviorFor<HtmlController>(x => x.Tag(null)).Outputs.Single().ShouldBeOfType<SparkHtmlTagOutput>();
        }

        [Test]
        public void should_replace_output_for_actions_returning_html_document_with_spark_document_output()
        {
            _graph.BehaviorFor<HtmlController>(x => x.Document(null)).Outputs.Single().ShouldBeOfType<SparkHtmlDocumentOutput>();
        }
    }

    public class HtmlController
    {
        public HtmlTag Tag(HtmlTagRequest request)
        {
            return new HtmlTag("body").Text("document body");
        }

        public HtmlDocument Document(HtmlDocumentRequest request)
        {
            var document = new HtmlDocument();
            document.Add(new HtmlTag("h1").Text("heading 1"));
            return document;
        }
    }

    public class HtmlDocumentRequest { }
    public class HtmlTagRequest { }

}