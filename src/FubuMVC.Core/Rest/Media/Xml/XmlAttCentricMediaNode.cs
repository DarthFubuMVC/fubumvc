using System.Xml;

namespace FubuMVC.Core.Rest.Media.Xml
{
    public class XmlAttCentricMediaNode : XmlMediaNode
    {
        public static XmlAttCentricMediaNode ForRoot(string rootElement)
        {
            return new XmlAttCentricMediaNode(new XmlDocument().WithRoot(rootElement));
        }

        public XmlAttCentricMediaNode(XmlElement element) : base(element)
        {
        }

        protected override IXmlMediaNode buildChildFor(XmlElement childElement)
        {
            return new XmlAttCentricMediaNode(childElement);
        }

        public override void SetAttribute(string name, object value)
        {
            Element.SetAttribute(name, value == null ? string.Empty : value.ToString());
        }
    }
}