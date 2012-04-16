using System.Xml;

namespace FubuMVC.Media.Xml
{
    public class XmlNodeCentricMediaNode : XmlMediaNode
    {
        public XmlNodeCentricMediaNode(XmlElement element) : base(element)
        {
        }

        public static XmlNodeCentricMediaNode ForRoot(string rootElement)
        {
            return new XmlNodeCentricMediaNode(new XmlDocument().WithRoot(rootElement));
        }

        protected override IXmlMediaNode buildChildFor(XmlElement childElement)
        {
            return new XmlNodeCentricMediaNode(childElement);
        }

        public override void SetAttribute(string name, object value)
        {
            if (value != null) Element.AddElement(name).InnerText = value.ToString();
        }
    }
}