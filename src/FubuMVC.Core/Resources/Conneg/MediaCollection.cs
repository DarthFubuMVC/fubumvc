using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg
{
    public class MediaCollection<T> : IMediaCollection<T> where T : class
    {
        private readonly IEnumerable<IMedia<T>> _media;

        public MediaCollection(IEnumerable<IMedia<T>> media)
        {
            _media = media;
        }

        public MediaCollection(IEnumerable<IMedia> media)
        {
            _media = media.OfType<IMedia<T>>().ToArray();
        } 

        public IEnumerable<IMedia<T>> Media
        {
            get { return _media; }
        }

        public IMedia<T> SelectMedia(CurrentMimeType mimeTypes, IFubuRequestContext context)
        {
            foreach (var acceptType in mimeTypes.AcceptTypes)
            {
                var candidates = _media.Where(x => x.Mimetypes.Contains(acceptType));
                if (candidates.Any())
                {
                    var writer = candidates.FirstOrDefault(x => x.MatchesRequest(context));
                    if (writer != null)
                    {
                        context.Logger.DebugMessage(() => new WriterChoice(acceptType, writer, writer.Condition));
                        return writer;
                    }

                    context.Logger.DebugMessage(() => NoWritersMatch.For(acceptType, candidates));
                }
            }

            if (mimeTypes.AcceptsAny())
            {
                var media = _media.FirstOrDefault(x => x.MatchesRequest(context));
                context.Logger.DebugMessage(() => new WriterChoice(MimeType.Any.Value, media, media.Condition));

                return media;
            }

            return null;
        }
    }
}