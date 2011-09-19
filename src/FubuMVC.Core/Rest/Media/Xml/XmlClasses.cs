using System;
using System.Collections.Generic;
using System.Xml;
using FubuMVC.Core.Runtime;
using FubuCore;

namespace FubuMVC.Core.Rest.Media.Xml
{
    public class XmlMediaDocument : IMediaDocument
    {
        private readonly XmlDocument _document;
        private readonly IXmlMediaNode _topNode;
        private XmlMediaOptions _options;

        public XmlMediaDocument(XmlMediaOptions options)
        {
            _document = new XmlDocument();
            if (options.Namespace.IsEmpty())
            {
                _document.WithRoot(options.Root);
            }
            else
            {
                var node = _document.CreateNode(XmlNodeType.Element, options.Root, options.Namespace);
                _document.AppendChild(node);
            }


            _topNode = options.NodeStyle == XmlNodeStyle.AttributeCentric
                           ? (IXmlMediaNode) new XmlAttCentricMediaNode(_document.DocumentElement)
                           : new XmlNodeCentricMediaNode(_document.DocumentElement);

            _topNode.LinkWriter = options.LinkWriter;

            _options = options;
        }

        public IMediaNode Root
        {
            get
            {
                return _topNode;
            }
        }

        public void Write(IOutputWriter writer)
        {
            writer.Write(_options.Mimetype, _document.OuterXml);
        }
    }

    public interface IXmlMediaNode : IMediaNode
    {
        XmlElement Element { get;}
        IXmlLinkWriter LinkWriter { get; set;}
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
            Mimetype = "text/xml";
        }

        public string Namespace { get; set; }
        public string Root { get; set; }
        public XmlNodeStyle NodeStyle { get; set; }
        public IXmlLinkWriter LinkWriter { get; set; }

        public string Mimetype { get; set; }
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