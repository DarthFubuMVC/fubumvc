using System.Collections.Generic;
using FubuMVC.Core.Resources.Media.Projections;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Resources.Media
{
    public class MediaWriter<T> : IMediaWriter<T>
    {
        private readonly IMediaDocument _document;
        private readonly ILinkSource<T> _links;
        private readonly IValueProjection<T> _projection;
        private readonly IUrlRegistry _urls;

        public MediaWriter(IMediaDocument document, ILinkSource<T> links, IUrlRegistry urls,
                           IValueProjection<T> projection)
        {
            _document = document;
            _links = links;
            _urls = urls;
            _projection = projection;
        }

        protected IMediaDocument document
        {
            get { return _document; }
        }

        public void Write(IValues<T> source, IOutputWriter writer)
        {
            writeData(source);

            _document.Write(writer);
        }

        public void Write(T source, IOutputWriter writer)
        {
            Write(new SimpleValues<T>(source), writer);
        }

        public IEnumerable<string> Mimetypes
        {
            get { return _document.Mimetypes; }
        }

        protected void writeData(IValues<T> source)
        {
            var links = _links.LinksFor(source, _urls);
            var topNode = _document.Root;
            topNode.WriteLinks(links);

            _projection.WriteValue(source, topNode);
        }
    }
}