namespace FubuMVC.Core.Rest.Media.Xml
{
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
}