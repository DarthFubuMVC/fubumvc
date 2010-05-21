using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class OutputColumnTester
    {
        [Test]
        public void write_with_a_single_output()
        {
            var chain = new BehaviorChain();
            var node = new RenderJsonNode(typeof (RouteInput));
            chain.AddToEnd(node);

            var tag = new HtmlTag("td");

            var column = new OutputColumn();
            column.WriteBody(chain, null, tag);

            tag.Text().ShouldEqual(node.Description);
        }

        [Test]
        public void write_with_multiple_outputs()
        {
            var chain = new BehaviorChain();
            var json = new RenderJsonNode(typeof (RouteInput));
            chain.AddToEnd(json);

            var text = new RenderTextNode<RouteInput>();
            chain.AddToEnd(text);

            var tag = new HtmlTag("td");

            var column = new OutputColumn();
            column.WriteBody(chain, null, tag);

            tag.Text().ShouldEqual(json.Description + ", " + text.Description);
        }

        [Test]
        public void write_with_no_outputs()
        {
            var chain = new BehaviorChain();
            var tag = new HtmlTag("td");

            var column = new OutputColumn();
            column.WriteBody(chain, null, tag);

            tag.Text().ShouldEqual(" -");
        }
    }
}