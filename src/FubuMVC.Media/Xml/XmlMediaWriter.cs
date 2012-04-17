using System.Xml;
using FubuCore;
using FubuMVC.Core.Urls;
using FubuMVC.Media.Projections;

namespace FubuMVC.Media.Xml
{
    public class XmlMediaWriter<T> : MediaWriter<T>, IXmlMediaWriter<T>
    {
        public XmlMediaWriter(XmlMediaOptions options, ILinkSource<T> links, IUrlRegistry urls,
                              IProjection<T> projection)
            : base(new XmlMediaDocument(options), links, urls, projection, null, null)
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