using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Diagnostics.HtmlWriting.Columns;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Resources.Conneg;
using FubuTestingSupport;
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
            var node = new OutputNode(typeof (RouteParameter));
            chain.AddToEnd(node);

            var tag = new HtmlTag("td");

            var column = new OutputColumn();
            column.WriteBody(chain, null, tag);

            tag.Text().ShouldEqual(node.Description);
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