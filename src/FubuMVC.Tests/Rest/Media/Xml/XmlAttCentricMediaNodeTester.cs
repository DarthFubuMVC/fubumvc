using FubuMVC.Core.Rest.Media.Xml;
using FubuTestingSupport;
using NUnit.Framework;
using FubuCore;

namespace FubuMVC.Tests.Rest.Media.Xml
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
    }
}