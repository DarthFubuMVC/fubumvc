using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Media.Xml;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Media.Testing.Xml
{
    [TestFixture]
    public class XmlMediaDocumentTester
    {
        [Test]
        public void create_media_document_with_Att_centric_style_builds_att_centric_nodes()
        {
            var document = new XmlMediaDocument(new XmlMediaOptions{
                NodeStyle = XmlNodeStyle.AttributeCentric
            });

            document.Root.ShouldBeOfType<XmlAttCentricMediaNode>();
        }

        [Test]
        public void create_media_document_with_node_centric_style_builds_node_centric_nodes()
        {
            var document = new XmlMediaDocument(new XmlMediaOptions{
                NodeStyle = XmlNodeStyle.NodeCentric
            });

            document.Root.ShouldBeOfType<XmlNodeCentricMediaNode>();
        }

        [Test]
        public void the_root_element_should_match_the_option_root()
        {
            var options = new XmlMediaOptions{
                NodeStyle = XmlNodeStyle.AttributeCentric,
                Root = "Something",
                Namespace = "http://mynamespace.xsd"
            };
            var document = new XmlMediaDocument(options);

            var root = document.Root.ShouldBeOfType<XmlMediaNode>().Element;

            root.Name.ShouldEqual(options.Root);
        }

        [Test]
        public void root_element_with_a_namespace()
        {
            var options = new XmlMediaOptions
            {
                NodeStyle = XmlNodeStyle.AttributeCentric,
                Root = "Something",
                Namespace = "http://mynamespace.xsd"
            };
            var document = new XmlMediaDocument(options);

            var root = document.Root.ShouldBeOfType<XmlMediaNode>().Element;

            root.OuterXml.ShouldEqual("<Something xmlns=\"http://mynamespace.xsd\" />");
        }

        [Test]
        public void the_root_has__the_link_writer_from_the_options()
        {
            var options = new XmlMediaOptions{
                NodeStyle = XmlNodeStyle.AttributeCentric,
                Root = "Something",
                LinkWriter = new StubLinkWriter()
            };

            var document = new XmlMediaDocument(options);

            var root = document.Root.ShouldBeOfType<XmlMediaNode>();
            root.LinkWriter.ShouldBeTheSameAs(options.LinkWriter);
        }

        [Test]
        public void write_should_use_the_mime_type_from_the_options()
        {
            var writer = MockRepository.GenerateMock<IOutputWriter>();

            var options = new XmlMediaOptions
            {
                NodeStyle = XmlNodeStyle.AttributeCentric,
                Root = "Something",
                LinkWriter = new StubLinkWriter()
            };

            var document = new XmlMediaDocument(options);

            document.Write(writer, "text/xml");

            writer.AssertWasCalled(x => x.Write(options.Mimetype, document.Root.As<XmlMediaNode>().Element.OuterXml));
        }

        [Test]
        public void should_list_the_mime_type_from_the_xml_options()
        {
            var options = new XmlMediaOptions
            {
                NodeStyle = XmlNodeStyle.AttributeCentric,
                Root = "Something",
                LinkWriter = new StubLinkWriter(),
                Mimetype = "vnd.dovetail.resource"
            };

            var document = new XmlMediaDocument(options);
            document.Mimetypes.Single().ShouldEqual(options.Mimetype);
        }

        [Test]
        public void should_list_the_mime_type_from_the_xml_options_if_there_are_multiples()
        {
            var options = new XmlMediaOptions
            {
                NodeStyle = XmlNodeStyle.AttributeCentric,
                Root = "Something",
                LinkWriter = new StubLinkWriter(),
                Mimetype = "vnd.dovetail.resource,text/xml,application/xml"
            };

            var document = new XmlMediaDocument(options);
            document.Mimetypes.ShouldHaveTheSameElementsAs(options.Mimetype.ToDelimitedArray(','));
        }
    }


    public class StubLinkWriter : IXmlLinkWriter
    {
        public void Write(XmlElement parent, IEnumerable<Link> links)
        {
            throw new NotImplementedException();
        }
    }
}