using System.Xml;
using FubuMVC.Media.Projections;

namespace FubuMVC.Media.Xml
{
    public interface IXmlMediaNode : IMediaNode
    {
        XmlElement Element { get; }
        IXmlLinkWriter LinkWriter { get; set; }
    }
}