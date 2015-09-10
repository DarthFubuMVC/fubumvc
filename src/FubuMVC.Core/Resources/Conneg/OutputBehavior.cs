using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg
{
    // This is mostly tested through integration tests
    public class OutputBehavior<T> : IActionBehavior where T : class
    {
        private readonly IFubuRequestContext _context;
        private readonly IOutputNode _media;
        private readonly IResourceNotFoundHandler _notFoundHandler;

        public OutputBehavior(IFubuRequestContext context, IOutputNode media, IResourceNotFoundHandler notFoundHandler) 
        {
            _context = context;
            _media = media;
            _notFoundHandler = notFoundHandler;
        }

        public IActionBehavior InsideBehavior { get; set; }



        public void Invoke()
        {
            if (InsideBehavior != null) InsideBehavior.Invoke();
            Write();
        }

        public void InvokePartial()
        {
            if (InsideBehavior != null) InsideBehavior.InvokePartial();

            if (shouldWriteInPartial())
            {
                Write();
            }
        }

        private bool shouldWriteInPartial()
        {
            if (!_context.Models.Has<OutputPartialBehavior>()) return true;

            return _context.Models.Get<OutputPartialBehavior>() == OutputPartialBehavior.Write;
        }

        // SAMPLE: output-behavior-mechanics
        public virtual void Write()
        {
            // If the resource is NOT found, return 
            // invoke the 404 handler
            var resource = _context.Models.Get<T>();
            if (resource == null)
            {
                _context.Writer.WriteResponseCode(HttpStatusCode.NotFound);
                _notFoundHandler.HandleResourceNotFound<T>();

                return;
            }

            // Resolve our CurrentMimeType object from the 
            // HTTP request that we use to represent
            // the mimetypes of the current request
            var mimeTypes = _context.Models.Get<CurrentMimeType>();

            WriteResource(mimeTypes, resource);

            // Write any output headers exposed by the IHaveHeaders
            // interface on the resource type
            WriteHeaders();
        }

        public void WriteResource(CurrentMimeType mimeTypes, T resource)
        {
            // Select the appropriate media writer
            // based on the mimetype and other runtime
            // conditions
            var media = _media.ChooseOutput<T>(mimeTypes.AcceptTypes.Raw);

            if (media == null)
            {
                // If no matching media can be found, write HTTP 406
                _context.Writer.WriteResponseCode(HttpStatusCode.NotAcceptable);
                _context.Writer.Write(MimeType.Text, "406:  Not acceptable");
            }
            else
            {
                _context.Logger.DebugMessage(() => new WriterChoice(media.MimeType, media.Writer));

                // Write the media based on a matching media type
                media.Writer.Write(media.MimeType, _context, resource);
            }
        }

        // ENDSAMPLE

        public IEnumerable<IMediaWriter<T>> Media
        {
            get { return _media.Media<T>(); }
        }

        public void WriteHeaders()
        {
            _context.Models.Find<IHaveHeaders>()
                .SelectMany(x => x.Headers)
                .Each(x => x.Write(_context.Writer));
        }




    }
}