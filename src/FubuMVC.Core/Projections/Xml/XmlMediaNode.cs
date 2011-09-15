using System.Xml;

namespace FubuMVC.Core.Projections.Xml
{
    // TODO -- need to do a node-centric approach too.
    public class XmlMediaNode : IMediaNode
    {
        public static XmlMediaNode ForRoot(string rootElement)
        {
            return new XmlMediaNode(new XmlDocument().WithRoot(rootElement));
        }

        private readonly XmlElement _element;

        public XmlMediaNode(XmlElement element)
        {
            _element = element;
        }

        public IMediaNode AddChild(string name)
        {
            return new XmlMediaNode(_element.AddElement(name));
        }

        public void SetAttribute(string name, object value)
        {
            _element.SetAttribute(name, value == null ? string.Empty : value.ToString());
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