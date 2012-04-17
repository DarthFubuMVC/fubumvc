using FubuMVC.Media.Xml;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Media.Testing.Xml
{
    [TestFixture]
    public class XmlMediaOptionsTester
    {
        private XmlMediaOptions theDefaultOptions;

        [SetUp]
        public void SetUp()
        {
            theDefaultOptions = new XmlMediaOptions();
        }

        [Test]
        public void mime_type_is_xml_by_default()
        {
            theDefaultOptions.Mimetype.ShouldEqual("text/xml,application/xml");
        }

        [Test]
        public void root_is_root()
        {
            theDefaultOptions.Root.ShouldEqual("Root");
        }

        [Test]
        public void namespace_is_null()
        {
            theDefaultOptions.Namespace.ShouldBeNull();
        }

        [Test]
        public void Nodestyle_is_node_based()
        {
            theDefaultOptions.NodeStyle.ShouldEqual(XmlNodeStyle.NodeCentric);
        }

        [Test]
        public void link_style_is_from_atom_writer()
        {
            theDefaultOptions.LinkWriter.ShouldBeTheSameAs(AtomXmlLinkWriter.Flyweight);
        }
    }
}