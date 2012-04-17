using FubuMVC.Media.Xml;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Media.Testing.Xml
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

        [Test]
        public void writing_a_list()
        {
            var node = XmlNodeCentricMediaNode.ForRoot("root");
            var list = node.AddList("node", "leaf");
            list.Add().SetAttribute("name", "Rand");
            list.Add().SetAttribute("name", "Perrin");
            list.Add().SetAttribute("name", "Mat");

            node.Element.OuterXml.ShouldEqual("<root><node><leaf><name>Rand</name></leaf><leaf><name>Perrin</name></leaf><leaf><name>Mat</name></leaf></node></root>");

        }
    }
}