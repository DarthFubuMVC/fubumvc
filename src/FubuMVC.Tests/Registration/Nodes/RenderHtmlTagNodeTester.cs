using FubuMVC.Core.Registration.Nodes;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Nodes
{
    [TestFixture]
    public class RenderHtmlTagNodeTester
    {
        private RenderHtmlTagNode node;

        [SetUp]
        public void SetUp()
        {
            node = new RenderHtmlTagNode();
        }

        [Test]
        public void description_should_return_write_html_tag()
        {
            node.Description.ShouldEqual("Write HtmlTag");
        }
    }
}