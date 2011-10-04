using System.Xml;

namespace FubuMVC.Core.Resources.Media.Xml
{
    public interface IXmlMediaNode : IMediaNode
    {
        XmlElement Element { get; }
        IXmlLinkWriter LinkWriter { get; set; }
    }
}