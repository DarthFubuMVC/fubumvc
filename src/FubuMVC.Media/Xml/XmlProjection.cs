using System;
using System.Linq.Expressions;
using FubuMVC.Core.Urls;
using FubuMVC.Media.Projections;

namespace FubuMVC.Media.Xml
{
    public class XmlProjection<T> : XmlMediaOptions, IXmlMediaWriterSource<T>
    {
        private readonly Projection<T> _projection = new Projection<T>(DisplayFormatting.RawValues);

        public XmlProjection()
        {
            Root = typeof (T).Name;
        }

        IXmlMediaWriter<T> IXmlMediaWriterSource<T>.BuildWriter()
        {
            return new XmlMediaWriter<T>(this, new LinksSource<T>(), new StubUrlRegistry(), _projection);
        }

        IXmlMediaWriter<T> IXmlMediaWriterSource<T>.BuildWriterFor(ILinkSource<T> links, IUrlRegistry urls)
        {
            return new XmlMediaWriter<T>(this, links, urls, _projection);
        }

        public AccessorProjection<T, TValue> Value<TValue>(Expression<Func<T, TValue>> expression)
        {
            return _projection.Value(expression);
        }
    }
}