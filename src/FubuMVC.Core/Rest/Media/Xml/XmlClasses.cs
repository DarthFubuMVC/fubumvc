using System;
using System.Collections.Generic;
using System.Xml;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Rest.Media.Xml
{
    public class XmlMediaDocument : IMediaDocument
    {
        public IMediaNode CreateRoot()
        {
            throw new NotImplementedException();
        }

        public void Write(IOutputWriter writer)
        {
            throw new NotImplementedException();
        }
    }

    public enum XmlNodeStyle
    {
        AttributeCentric,
        NodeCentric
    }

    public class XmlMediaOptions
    {
        public XmlMediaOptions()
        {
            Root = "Root";
            NodeStyle = XmlNodeStyle.NodeCentric;
            LinkWriter = AtomXmlLinkWriter.Flyweight;
        }

        public string Namespace { get; set; }
        public string Root { get; set; }
        public XmlNodeStyle NodeStyle { get; set; }
        public IXmlLinkWriter LinkWriter { get; set; }
    }

    public interface IXmlLinkWriter
    {
        void Write(XmlElement parent, IEnumerable<Link> links);
    }

    public class AtomXmlLinkWriter : IXmlLinkWriter
    {
        public static readonly AtomXmlLinkWriter Flyweight = new AtomXmlLinkWriter();

        public void Write(XmlElement parent, IEnumerable<Link> links)
        {
            throw new NotImplementedException();
        }
    }
}