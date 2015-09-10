using System.Collections.Generic;
using System.Linq;
using System.Net;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg
{
    public class InputBehavior<T> : BasicBehavior where T : class
    {
        private readonly IFubuRequestContext _context;
        private readonly IInputNode _readers;

        public InputBehavior(IFubuRequestContext context, IInputNode readers) : base(PartialBehavior.Executes)
        {
            _context = context;
            _readers = readers;
        }

        // SAMPLE: input-behavior-mechanics
        protected override DoNext performInvoke()
        {
            // Might already be there from a different way
            if (_context.Models.Has<T>()) return DoNext.Continue;

            
            var contentType = _context.Request.GetHeader(HttpRequestHeader.ContentType).FirstOrDefault() ??
                              MimeType.HttpFormMimetype;
            if (contentType.Contains(";"))
            {
                var parts = contentType.ToDelimitedArray(';');

                // TODO -- Do something with the charset someday
                contentType = parts.First();
                /*
                if (parts.Last().Contains("charset"))
                {
                    Charset = parts.Last().Split('=').Last();
                }
                 */
            }



            // Choose the first reader that says it
            // can read the mimetype from the 
            // 'content-type' header in the request
            var reader = _readers.SelectReader(contentType) as IReader<T>;

            

            if (reader == null)
            {
                // If we don't find a matching Reader for the
                // content-type of the request, this request fails
                // with an HTTP 415 return code
                failWithInvalidMimeType();
                return DoNext.Stop;
            }

            _context.Logger.DebugMessage(() => new ReaderChoice(contentType, reader));

            // Use the selected reader
            var target = reader.Read(contentType, _context);
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
}