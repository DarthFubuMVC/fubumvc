using System.Xml;
using FubuCore;

namespace FubuMVC.Media.Xml
{
    public static class XmlProjectionExtensions
    {
        public static XmlDocument Write<T>(this XmlProjection<T> projection, T subject)
        {
            return projection.As<IXmlMediaWriterSource<T>>().BuildWriter().WriteSubject(subject);
        }
    }
}