using System.Linq;
using System.Net;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Logging;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg
{
    public class InputBehavior<T> : BasicBehavior where T : class
    {
        private readonly IFubuRequestContext _context;
        private readonly IReaderCollection<T> _readers;

        public InputBehavior(IFubuRequestContext context, IReaderCollection<T> readers) : base(PartialBehavior.Executes)
        {
            _context = context;
            _readers = readers;
        }

        // SAMPLE: input-behavior-mechanics
        protected override DoNext performInvoke()
        {
            // Might already be there from a different way
            if (_context.Models.Has<T>()) return DoNext.Continue;

            // Resolve our CurrentMimeType object from the 
            // HTTP request that we use to represent
            // the mimetypes of the current request
            var mimeTypes = _context.Models.Get<CurrentMimeType>();

            // Choose the first reader that says it
            // can read the mimetype from the 
            // 'content-type' header in the request
            var reader = _readers.ChooseReader(mimeTypes, _context);

            _context.Logger.DebugMessage(() => new ReaderChoice(mimeTypes, reader));

            if (reader == null)
            {
                // If we don't find a matching Reader for the
                // content-type of the request, this request fails
                // with an HTTP 415 return code
                failWithInvalidMimeType();
                return DoNext.Stop;
            }

            // Use the selected reader
            var target = reader.Read(mimeTypes.ContentType, _context);
            _context.Models.Set(target);

            return DoNext.Continue;
        }

        // ENDSAMPLE

        private void failWithInvalidMimeType()
        {
            _context.Writer.WriteResponseCode(HttpStatusCode.UnsupportedMediaType);
            _context.Writer.Write(MimeType.Text, "415:  Unsupported Media Type");
        }
    }

    public class ReaderChoice : LogRecord, DescribesItself
    {
        private readonly CurrentMimeType _mimeType;
        private readonly Description _reader;

        public ReaderChoice(CurrentMimeType mimeType, object reader)
        {
            _mimeType = mimeType;
            if (reader != null) _reader = Description.For(reader);
        }

        public void Describe(Description description)
        {
            description.Title = _reader == null
                ? "Unable to select a reader for content-type '{0}'".ToFormat(_mimeType.ContentType)
                : "Selected reader '{0}' for content-type '{1}'".ToFormat(_reader.Title, _mimeType.ContentType);
        }
    }
}