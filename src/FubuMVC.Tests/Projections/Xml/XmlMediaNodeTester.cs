using NUnit.Framework;

namespace FubuMVC.Tests.Projections.Xml
{
    [TestFixture]
    public class XmlMediaNodeTester
    {
        [Test]
        public void write_simple_attribute()
        {
            var node = XmlMediaNode.ForRoot("root");
            node.SetAttribute("a", "1");

            node.Element.GetAttribute("a").ShouldEqual("1");
        }

        [Test]
        public void write_null_value_as_empty_string()
        {
            var node = XmlMediaNode.ForRoot("root");
            node.SetAttribute("a", null);
        }

        [Test]
        public void add_simple_child_to_original_media_node()
        {
            var node = XmlMediaNode.ForRoot("root");
            node.AddChild("childA");
            node.AddChild("childB");

            node.Element.ChildNodes.Count.ShouldEqual(2);
            node.Element.FirstChild.Name.ShouldEqual("childA");
            node.Element.LastChild.Name.ShouldEqual("childB");
        }
    }
}