using System.Xml;

namespace FubuMVC.Core.Rest.Media.Xml
{
    public interface IXmlMediaNode : IMediaNode
    {
        XmlElement Element { get;}
        IXmlLinkWriter LinkWriter { get; set;}
    }
}