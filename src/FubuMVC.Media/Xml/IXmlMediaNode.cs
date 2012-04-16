using System.Xml;
using FubuMVC.Core.Projections;

namespace FubuMVC.Media.Xml
{
    public interface IXmlMediaNode : IMediaNode
    {
        XmlElement Element { get; }
        IXmlLinkWriter LinkWriter { get; set; }
    }
}