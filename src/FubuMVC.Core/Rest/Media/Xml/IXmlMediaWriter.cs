using System.Xml;

namespace FubuMVC.Core.Rest.Media.Xml
{
    public interface IXmlMediaWriter<T>
    {
        XmlDocument WriteValues(IValues<T> values);
        XmlDocument WriteSubject(T subject);
    }
}