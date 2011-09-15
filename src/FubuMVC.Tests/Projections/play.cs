using System;
using System.Diagnostics;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using NUnit.Framework;
using FubuCore.Configuration;

namespace FubuMVC.Tests.Projections
{
    [TestFixture]
    public class play
    {
        [Test]
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
    }
}