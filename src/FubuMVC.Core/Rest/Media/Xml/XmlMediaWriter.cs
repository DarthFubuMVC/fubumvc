using System.Xml;
using FubuCore;
using FubuMVC.Core.Rest.Media.Projections;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Rest.Media.Xml
{
    public class XmlMediaWriter<T> : MediaWriter<T>, IXmlMediaWriter<T>
    {
        public XmlMediaWriter(XmlMediaOptions options, ILinkSource<T> links, IUrlRegistry urls,
                              IValueProjection<T> projection)
            : base(new XmlMediaDocument(options), links, urls, projection)
        {
        }

        public XmlDocument Write(IValues<T> values)
        {
            writeData(values);

            return document.As<XmlMediaDocument>().Document;
        }

        public XmlDocument Write(T subject)
        {
            return Write(new SimpleValues<T>(subject));
        }
    }
}