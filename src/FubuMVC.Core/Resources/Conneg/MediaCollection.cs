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
        private readonly IOutputNode _node;

        public MediaCollection(IOutputNode node)
        {
            _node = node;
            _media = new Lazy<IEnumerable<IMediaWriter<T>>>(() => node.Media<T>().ToArray());
        }

        public IEnumerable<IMediaWriter<T>> Writers
        {
            get { return _media.Value; }
        }

        public IMediaWriter<T> SelectWriter(CurrentMimeType mimeTypes, IFubuRequestContext context)
        {
            var choice = _node.ChooseOutput<T>(mimeTypes.AcceptTypes.Raw);
            if (choice != null)
            {
                context.Logger.DebugMessage(() => new WriterChoice(choice.MimeType, choice.Writer));

                return choice.Writer;
            }

            return null;
        }
    }


}