using System.Xml;

namespace FubuMVC.Core.Resources.Media.Xml
{
    public interface IXmlMediaWriter<T>
    {
        XmlDocument WriteValues(IValues<T> values);
        XmlDocument WriteSubject(T subject);
    }
}