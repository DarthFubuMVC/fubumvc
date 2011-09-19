using System;
using System.Collections.Generic;
using System.Xml;

namespace FubuMVC.Core.Rest.Media.Xml
{
    public abstract class XmlMediaNode : IXmlMediaNode
    {
        private readonly XmlElement _element;

        protected XmlMediaNode(XmlElement element)
        {
            _element = element;
            LinkWriter = AtomXmlLinkWriter.Flyweight;
        }

        public IMediaNode AddChild(string name)
        {
            var childElement = _element.AddElement(name);
            var childNode = buildChildFor(childElement);
            childNode.LinkWriter = LinkWriter;

            return childNode;
        }

        protected abstract IXmlMediaNode buildChildFor(XmlElement childElement);

        public abstract void SetAttribute(string name, object value);

        public void WriteLinks(IEnumerable<Link> links)
        {
            LinkWriter.Write(_element, links);
        }

        public XmlElement Element
        {
            get { return _element; }
        }

        public IXmlLinkWriter LinkWriter { get; set; }

        public override string ToString()
        {
            return _element.OuterXml;
        }
    }
}