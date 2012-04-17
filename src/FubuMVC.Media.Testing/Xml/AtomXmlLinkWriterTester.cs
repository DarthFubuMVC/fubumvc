using System.Xml;
using FubuCore;
using FubuMVC.Media.Xml;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Media.Testing.Xml
{
    [TestFixture]
    public class AtomXmlLinkWriterTester
    {
        private Link theLink;

        [SetUp]
        public void SetUp()
        {
            theLink = new Link("http://something/else");
        }

        private string theResultingXml
        {
            get
            {
                var root = XmlExtensions.WithRoot(new XmlDocument(), "root");
                var writer = new AtomXmlLinkWriter();
                writer.Write(root, new Link[]{theLink});

                return root.FirstChild.OuterXml;
            }
        }

        [Test]
        public void write_link_with_only_the_href()
        {
            theResultingXml.ShouldEqual("<link href=\"{0}\" />".ToFormat(theLink.Url));
        }

        [Test]
        public void write_link_with_href_and_title()
        {
            theLink.Title = "some title";
            theResultingXml.ShouldEqual("<link href=\"{0}\" title=\"{1}\" />".ToFormat(theLink.Url, theLink.Title));
        }

        [Test]
        public void write_link_with_href_and_rel()
        {
            theLink.Rel = "something";

            theResultingXml.ShouldEqual("<link href=\"{0}\" rel=\"{1}\" />".ToFormat(theLink.Url, theLink.Rel));
        }

        [Test]
        public void write_link_with_href_and_type()
        {
            theLink.ContentType = "text/xml";
            theResultingXml.ShouldEqual("<link href=\"{0}\" type=\"{1}\" />".ToFormat(theLink.Url, theLink.ContentType));
        }

    }
}