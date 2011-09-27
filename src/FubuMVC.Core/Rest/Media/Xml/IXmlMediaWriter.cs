using System.Xml;

namespace FubuMVC.Core.Rest.Media.Xml
{
    public interface IXmlMediaWriter<T>
    {
        XmlDocument Write(IValues<T> values);
        XmlDocument Write(T subject);
    }
}