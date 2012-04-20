using System.Collections.Generic;
using System.Linq;
using System.Net;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg.New
{
    public class OutputBehavior<T> : BasicBehavior where T : class
    {
        private readonly IFubuRequest _request;
        private readonly IOutputWriter _writer;
        private readonly IEnumerable<IMedia<T>> _media;

        public OutputBehavior(IFubuRequest request, IOutputWriter writer, IEnumerable<IMedia<T>> media) : base(PartialBehavior.Executes)
        {
            _request = request;
            _writer = writer;
            _media = media;
        }

        protected override DoNext performInvoke()
        {
            Write();

            return DoNext.Continue;
        }

        // TODO -- Runtime tracing
        public void Write()
        {
            var mimeTypes = _request.Get<CurrentMimeType>();
            var media = SelectMedia(mimeTypes);

            if (media == null)
            {
                // TODO -- better error message?
                _writer.Write(MimeType.Text, "406:  Not acceptable");
                _writer.WriteResponseCode(HttpStatusCode.NotAcceptable);
            }
            else
            {
                var resource = _request.Get<T>();
                var outputMimetype = mimeTypes.SelectFirstMatching(media.Mimetypes);
                media.Write(outputMimetype, resource);
            }
        }

        public virtual IMedia<T> SelectMedia(CurrentMimeType mimeTypes)
        {
            foreach (var acceptType in mimeTypes.AcceptTypes)
            {
                var media = _media.FirstOrDefault(x => x.Mimetypes.Contains(acceptType) && x.MatchesRequest());
                if (media != null) return media;
            }

            if (mimeTypes.AcceptsAny())
            {
                return _media.FirstOrDefault(x => x.MatchesRequest());
            }

            return null;
        }

        public IEnumerable<IMedia<T>> Media
        {
            get { return _media; }
        }
    }
}