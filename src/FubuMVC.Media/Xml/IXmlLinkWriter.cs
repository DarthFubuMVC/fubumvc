using System.Collections.Generic;
using System.Xml;

namespace FubuMVC.Media.Xml
{
    public interface IXmlLinkWriter
    {
        void Write(XmlElement parent, IEnumerable<Link> links);
    }
}