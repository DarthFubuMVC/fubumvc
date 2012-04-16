using System.Collections.Generic;
using System.Xml;
using FubuCore;
using FubuCore.Configuration;

namespace FubuMVC.Media.Xml
{
    public class AtomXmlLinkWriter : IXmlLinkWriter
    {
        public static readonly AtomXmlLinkWriter Flyweight = new AtomXmlLinkWriter();

        public void Write(XmlElement parent, IEnumerable<Link> links)
        {
            links.Each(x => WriteLink(parent, x));
        }

        public void WriteLink(XmlElement parent, Link link)
        {
            var element = parent.AddElement("link").WithAtt("href", link.Url);


            link.Title.IfNotNull(x => element.WithAtt("title", x));
            link.Rel.IfNotNull(x => element.WithAtt("rel", x));
            link.ContentType.IfNotNull(x => element.WithAtt("type", x));
        }
    }
}