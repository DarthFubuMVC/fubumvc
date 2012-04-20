using System.Collections.Generic;
using System.Linq;
using System.Net;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg.New
{
    public class InputBehavior<T> : BasicBehavior where T : class
    {
        private readonly IOutputWriter _writer;
        private readonly IFubuRequest _request;
        private readonly IEnumerable<IReader<T>> _readers;

        public InputBehavior(IOutputWriter writer, IFubuRequest request, IEnumerable<IReader<T>> readers) : base(PartialBehavior.Executes)
        {
            _writer = writer;
            _request = request;
            _readers = readers;
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

            var target = reader.Read(mimeTypes.ContentType);
            _request.Set(target);

            return DoNext.Continue;
        }

        private void failWithInvalidMimeType()
        {
            _writer.Write(MimeType.Text, "415:  Unsupported Media Type");
            _writer.WriteResponseCode(HttpStatusCode.UnsupportedMediaType);
        }

        public IReader<T> ChooseReader(CurrentMimeType mimeTypes)
        {
            return _readers.FirstOrDefault(x => x.Mimetypes.Contains(mimeTypes.ContentType));
        }
    }
}