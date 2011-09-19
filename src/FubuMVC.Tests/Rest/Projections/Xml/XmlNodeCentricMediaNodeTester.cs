using FubuMVC.Core.Rest.Projections.Xml;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Rest.Projections.Xml
{
    [TestFixture]
    public class XmlNodeCentricMediaNodeTester
    {
        [Test]
        public void write_a_simple_attribute()
        {
            var node = XmlNodeCentricMediaNode.ForRoot("root");
            node.SetAttribute("a", "1");

            node.ToString().ShouldEqual("<root><a>1</a></root>");
        }

        [Test]
        public void do_not_set_the_child_node_if_the_value_is_null()
        {
            var node = XmlNodeCentricMediaNode.ForRoot("root");
            node.SetAttribute("a", null);

            node.ToString().ShouldEqual("<root />");
        }

        [Test]
        public void add_child()
        {
            var node = XmlNodeCentricMediaNode.ForRoot("root");
            var child = node.AddChild("child");
            child.SetAttribute("a", "1");
            child.SetAttribute("b", "2");

            node.ToString().ShouldEqual("<root><child><a>1</a><b>2</b></child></root>");
        }
    }
}