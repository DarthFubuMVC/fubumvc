using System.Xml;
using FubuCore;
using FubuMVC.Core.Resources.Media.Projections;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Resources.Media.Xml
{
    public class XmlMediaWriter<T> : MediaWriter<T>, IXmlMediaWriter<T>
    {
        public XmlMediaWriter(XmlMediaOptions options, ILinkSource<T> links, IUrlRegistry urls,
                              IValueProjection<T> projection)
            : base(new XmlMediaDocument(options), links, urls, projection)
        {
        }

        public XmlDocument WriteValues(IValues<T> values)
        {
            writeData(values);

            return document.As<XmlMediaDocument>().Document;
        }

        public XmlDocument WriteSubject(T subject)
        {
            return WriteValues(new SimpleValues<T>(subject));
        }
    }
}