using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg
{
    public class MediaCollection<T> : IMediaCollection<T> where T : class
    {
        private readonly Lazy<IEnumerable<IMediaWriter<T>>> _media; 

        public MediaCollection(IOutputNode node)
        {
            _media = new Lazy<IEnumerable<IMediaWriter<T>>>(() => node.Media<T>().ToArray());
        }

        public IEnumerable<IMediaWriter<T>> Writers
        {
            get { return _media.Value; }
        }

        public IMediaWriter<T> SelectWriter(CurrentMimeType mimeTypes, IFubuRequestContext context)
        {
            foreach (var acceptType in mimeTypes.AcceptTypes)
            {
                var candidate = Writers.FirstOrDefault(x => x.Mimetypes.Contains(acceptType));
                if (candidate != null)
                {
                    if (candidate != null)
                    {
                        context.Logger.DebugMessage(() => new WriterChoice(acceptType, candidate));
                        return candidate;
                    }
                }

                
            }

            if (mimeTypes.AcceptsAny())
            {
                var media = Writers.FirstOrDefault();
                context.Logger.DebugMessage(() => new WriterChoice(MimeType.Any.Value, media));

                return media;
            }

            return null;
        }
    }
}