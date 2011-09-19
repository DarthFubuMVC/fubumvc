using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;

namespace FubuMVC.Core.Rest.Media.Projections.Xml
{
    public class XmlNodeCentricMediaNode : IMediaNode
    {
        private readonly XmlNode _parentNode;

        public static XmlNodeCentricMediaNode ForRoot(string rootElement)
        {
            return new XmlNodeCentricMediaNode(new XmlDocument().WithRoot(rootElement));
        }

        private XmlNodeCentricMediaNode(XmlNode parentNode)
        {
            _parentNode = parentNode;
        }

        public IMediaNode AddChild(string name)
        {
            return new XmlNodeCentricMediaNode(_parentNode.AddElement(name));
        }

        public void SetAttribute(string name, object value)
        {
            if (value != null) _parentNode.AddElement(name).InnerText = value.ToString();
        }

        public void WriteLinks(IEnumerable<SyndicationLink> links)
        {
            throw new NotImplementedException();
        }

        public XmlNode ParentNode
        {
            get { return _parentNode; }
        }

        public override string ToString()
        {
            return _parentNode.OuterXml;
        }
    }
}