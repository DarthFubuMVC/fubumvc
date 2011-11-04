using System;
using System.Diagnostics;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using NUnit.Framework;
using FubuCore.Configuration;

namespace FubuMVC.Tests.Resources.Projections
{
    [TestFixture]
    public class play
    {
        [Test]
		[Platform(Exclude="Mono")]
        public void try_it_out()
        {
            var doc = new XmlDocument().WithRoot("root").WithAtt("a", "1");
            var feed = new SyndicationFeed("some feed", "cool feed", new Uri("http://elsewere.com"));

            var item = new SyndicationItem("the title", "some content", new Uri("http://somewhere.com"));
            item.ElementExtensions.Add(doc);

            feed.Items = new SyndicationItem[]{item};

            var builder = new StringBuilder();
            var writer = XmlWriter.Create(builder);
            var formatter = new Atom10FeedFormatter(feed);

            formatter.WriteTo(writer);
            writer.Close();

            Debug.WriteLine(builder);
        }

        [Test]
        public void try_media_xml_with_namespace()
        {
            var doc = new XmlDocument();
            var root = doc.CreateNode(XmlNodeType.Element, "root", "http://mynamespace.xsd");
            doc.AppendChild(root);

            root.AddElement("something").InnerText = "else";

            Debug.WriteLine(doc.DocumentElement.OuterXml);
        }
    }
}