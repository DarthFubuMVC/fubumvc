using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FubuCore.Descriptions;
using FubuCore.Logging;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;
using FubuCore;

namespace FubuMVC.Core.Resources.Conneg
{
    public class InputBehavior<T> : BasicBehavior where T : class
    {
        private readonly IOutputWriter _writer;
        private readonly IFubuRequest _request;
        private readonly IEnumerable<IReader<T>> _readers;
        private readonly ILogger _logger;

        public InputBehavior(IOutputWriter writer, IFubuRequest request, IEnumerable<IReader<T>> readers, ILogger logger) : base(PartialBehavior.Executes)
        {
            _writer = writer;
            _request = request;
            _readers = readers;
            _logger = logger;
        }

        protected override DoNext performInvoke()
        {
            // Might already be there from a different way
            if (_request.Has<T>()) return DoNext.Continue;

            var mimeTypes = _request.Get<CurrentMimeType>();

            
            var reader = ChooseReader(mimeTypes);
            _logger.DebugMessage(() => new ReaderChoice(mimeTypes, reader));

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
            _writer.WriteResponseCode(HttpStatusCode.UnsupportedMediaType);
            _writer.Write(MimeType.Text, "415:  Unsupported Media Type");
        }

        public IReader<T> ChooseReader(CurrentMimeType mimeTypes)
        {
            return _readers.FirstOrDefault(x => x.Mimetypes.Contains(mimeTypes.ContentType));
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