using System.Collections.Generic;
using System.Xml;
using FubuMVC.Media.Projections;

namespace FubuMVC.Media.Xml
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

        public abstract void SetAttribute(string name, object value);

        public void WriteLinks(IEnumerable<Link> links)
        {
            LinkWriter.Write(_element, links);
        }

        public IMediaNodeList AddList(string nodeName, string leafName)
        {
            var parentElement = _element.AddElement(nodeName);
            return new XmlMediaNodeList(this, parentElement, leafName);
        }

        public XmlElement Element
        {
            get { return _element; }
        }

        public IXmlLinkWriter LinkWriter { get; set; }
        protected abstract IXmlMediaNode buildChildFor(XmlElement childElement);

        public override string ToString()
        {
            return _element.OuterXml;
        }

        public class XmlMediaNodeList : IMediaNodeList
        {
            private readonly XmlMediaNode _parentNode;
            private readonly XmlElement _parentElement;
            private readonly string _childElementName;

            public XmlMediaNodeList(XmlMediaNode parentNode, XmlElement parentElement, string childElementName)
            {
                _parentNode = parentNode;
                _parentElement = parentElement;
                _childElementName = childElementName;
            }

            public IMediaNode Add()
            {
                var element = _parentElement.AddElement(_childElementName);
                return _parentNode.buildChildFor(element);
            }
        }
    }
}