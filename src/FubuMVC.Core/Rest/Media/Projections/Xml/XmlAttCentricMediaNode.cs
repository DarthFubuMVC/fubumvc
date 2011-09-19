using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;

namespace FubuMVC.Core.Rest.Media.Projections.Xml
{
    // TODO -- need to do a node-centric approach too.
    public class XmlAttCentricMediaNode : IMediaNode
    {
        public static XmlAttCentricMediaNode ForRoot(string rootElement)
        {
            return new XmlAttCentricMediaNode(new XmlDocument().WithRoot(rootElement));
        }

        private readonly XmlElement _element;

        public XmlAttCentricMediaNode(XmlElement element)
        {
            _element = element;
        }

        public IMediaNode AddChild(string name)
        {
            return new XmlAttCentricMediaNode(_element.AddElement(name));
        }

        public void SetAttribute(string name, object value)
        {
            _element.SetAttribute(name, value == null ? string.Empty : value.ToString());
        }

        public void WriteLinks(IEnumerable<SyndicationLink> links)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return _element.OuterXml;
        }

        public XmlElement Element
        {
            get { return _element; }
        }
    }
}