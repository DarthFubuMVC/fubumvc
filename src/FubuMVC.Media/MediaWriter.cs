using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Resources.Conneg.New;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Media.Projections;

namespace FubuMVC.Media
{
    public class MediaWriter<T> : IMediaWriter<T>
    {
        private readonly IMediaDocument _document;
        private readonly ILinkSource<T> _links;
        private readonly IProjection<T> _projection;
        private readonly IServiceLocator _services;
        private readonly IUrlRegistry _urls;
        private readonly IOutputWriter _writer;

        public MediaWriter(IMediaDocument document, ILinkSource<T> links, IUrlRegistry urls, IProjection<T> projection,
                           IServiceLocator services, IOutputWriter writer)
        {
            _document = document;
            _links = links;
            _urls = urls;
            _projection = projection;
            _services = services;
            _writer = writer;
        }

        protected IMediaDocument document
        {
            get { return _document; }
        }

        // TODO -- need some end to end testing on this monster
        public void Write(string mimeType, T resource)
        {
            writeData(new SimpleValues<T>(resource));

            _document.Write(_writer, mimeType);
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

            _projection.Write(new ProjectionContext<T>(_services, source), topNode);
        }
    }
}