using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Resources.Media;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg
{
    public class ConnegOutputBehavior<T> : BasicBehavior
    {
        private readonly IFubuRequest _request;
        private readonly IValueSource<T> _source;
        private readonly IOutputWriter _writer;
        private readonly IEnumerable<IMediaWriter<T>> _writers;

        public ConnegOutputBehavior(IValueSource<T> source, IFubuRequest request, IOutputWriter writer,
                                    IEnumerable<IMediaWriter<T>> writers) : base(PartialBehavior.Executes)
        {
            _source = source;
            _request = request;
            _writer = writer;
            _writers = writers;
        }

        protected override DoNext performInvoke()
        {
            var mimeTypes = _request.Get<CurrentMimeType>();
            if (mimeTypes.AcceptTypes.Contains(MediaTypeNames.Text.Html))
            {
                return DoNext.Continue;
            }

            var writer = SelectWriter(mimeTypes);
            if (writer == null)
            {
                _writer.WriteResponseCode(HttpStatusCode.NotAcceptable);
            }
            else
            {
                writer.Write(_source.FindValues(), _writer);
            }

            return DoNext.Stop;
        }

        public virtual IMediaWriter<T> SelectWriter(CurrentMimeType mimeTypes)
        {
            foreach (var acceptType in mimeTypes.AcceptTypes)
            {
                if (acceptType == "*/*")
                {
                    return _writers.FirstOrDefault(x => x.Mimetypes.Contains(MediaTypeNames.Text.Html))
                           ?? _writers.FirstOrDefault();
                }

                var writer = _writers.FirstOrDefault(x => x.Mimetypes.Contains(acceptType));
                if (writer != null) return writer;
            }

            return null;
        }
    }
}