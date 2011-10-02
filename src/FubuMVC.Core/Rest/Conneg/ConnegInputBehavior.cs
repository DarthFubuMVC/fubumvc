using System.Collections.Generic;
using System.Linq;
using System.Net;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Rest.Media;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Rest.Conneg
{
    public class ConnegInputBehavior<T> : BasicBehavior where T : class
    {
        private readonly IEnumerable<IMediaReader<T>> _readers;
        private readonly IEnumerable<IMediaWriter<T>> _mediaWriters;
        private readonly IFubuRequest _request;
        private readonly IOutputWriter _writer;

        public ConnegInputBehavior(IEnumerable<IMediaReader<T>> readers,IEnumerable<IMediaWriter<T>> mediaWriters, IOutputWriter writer, IFubuRequest request)
            : base(PartialBehavior.Executes)
        {
            _readers = readers;
            _mediaWriters = mediaWriters;
            _writer = writer;
            _request = request;
        }

        protected override DoNext performInvoke()
        {
            var mimeTypes = _request.Get<CurrentMimeType>();
            var reader = ChooseReader(mimeTypes);

            if (reader == null)
            {
                failWithInvalidMimeType();
                return DoNext.Stop;
            }

            if(!requestedMediaTypeIsAccepted(mimeTypes))
            {
                failWithNotAcceptable();
                return DoNext.Stop;
            }

            var target = reader.Read(mimeTypes.ContentType);
            _request.Set(target);

            return DoNext.Continue;
        }

        private void failWithInvalidMimeType()
        {
            _writer.WriteResponseCode(HttpStatusCode.UnsupportedMediaType);
        }

        private void failWithNotAcceptable()
        {
            _writer.WriteResponseCode(HttpStatusCode.NotAcceptable);
        }

        private bool requestedMediaTypeIsAccepted(CurrentMimeType mimeType)
        {
            if (mimeType.AcceptTypes.Matches(""))
                return true;

            return _mediaWriters.Any(x => x.Mimetypes.Any(y => mimeType.AcceptTypes.Contains(y)));
        }

        public IMediaReader<T> ChooseReader(CurrentMimeType mimeTypes)
        {
            return _readers.FirstOrDefault(x => x.Mimetypes.Contains(mimeTypes.ContentType));
        }
    }
}