using FubuCore;
using FubuMVC.Media.Xml;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Media.Testing.Xml
{
    [TestFixture]
    public class XmlAttCentricMediaNodeTester
    {
        [Test]
        public void write_simple_attribute()
        {
            var node = XmlAttCentricMediaNode.ForRoot("root");
            node.SetAttribute("a", "1");

            node.Element.GetAttribute("a").ShouldEqual("1");
        }

        [Test]
        public void write_null_value_as_empty_string()
        {
            var node = XmlAttCentricMediaNode.ForRoot("root");
            node.SetAttribute("a", null);
        }

        [Test]
        public void add_simple_child_to_original_media_node()
        {
            var node = XmlAttCentricMediaNode.ForRoot("root");
            node.AddChild("childA");
            node.AddChild("childB");

            node.Element.ChildNodes.Count.ShouldEqual(2);
            node.Element.FirstChild.Name.ShouldEqual("childA");
            node.Element.LastChild.Name.ShouldEqual("childB");
        }

        [Test]
        public void building_a_child_propogates_the_XmlLinkWriter()
        {
            var node = XmlAttCentricMediaNode.ForRoot("root");
            node.AddChild("childA").As<XmlMediaNode>().LinkWriter.ShouldBeTheSameAs(node.LinkWriter);
        }

        [Test]
        public void writing_a_list()
        {
            var node = XmlAttCentricMediaNode.ForRoot("root");
            var list = node.AddList("node", "leaf");
            list.Add().SetAttribute("name", "Rand");
            list.Add().SetAttribute("name", "Perrin");
            list.Add().SetAttribute("name", "Mat");

            node.Element.OuterXml.ShouldEqual("<root><node><leaf name=\"Rand\" /><leaf name=\"Perrin\" /><leaf name=\"Mat\" /></node></root>");
        
        }
    }
}