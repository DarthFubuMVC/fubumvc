namespace FubuMVC.Media.Xml
{
    public class XmlMediaOptions
    {
        public XmlMediaOptions()
        {
            Root = "Root";
            NodeStyle = XmlNodeStyle.NodeCentric;
            LinkWriter = AtomXmlLinkWriter.Flyweight;
            Mimetype = "text/xml,application/xml";
        }

        public string Namespace { get; set; }
        public string Root { get; set; }
        public XmlNodeStyle NodeStyle { get; set; }
        public IXmlLinkWriter LinkWriter { get; set; }

        /// <summary>
        /// Accepted mimetype's that this document writer
        /// will accept.  Can be comma delimited like:
        /// "text/xml,application/xml"
        /// </summary>
        public string Mimetype { get; set; }
    }
}