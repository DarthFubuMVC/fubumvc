using System.Xml;
using FubuMVC.Media.Projections;

namespace FubuMVC.Media.Xml
{
    public interface IXmlMediaWriter<T>
    {
        XmlDocument WriteValues(IValues<T> values);
        XmlDocument WriteSubject(T subject);
    }
}