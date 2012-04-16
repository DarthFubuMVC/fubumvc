using System.Xml;
using FubuMVC.Core.Projections;

namespace FubuMVC.Media.Xml
{
    public interface IXmlMediaWriter<T>
    {
        XmlDocument WriteValues(IValues<T> values);
        XmlDocument WriteSubject(T subject);
    }
}