using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FubuMVC.Core.Http.Compression
{
    public interface IHttpContentEncoders
    {
        IHttpContentEncoding MatchFor(string acceptEncoding);
        IEnumerable<IHttpContentEncoding> Encodings { get; }
    }

    public class HttpContentEncoders : IHttpContentEncoders
    {
        private readonly IEnumerable<IHttpContentEncoding> _encodings;

        public HttpContentEncoders(IEnumerable<IHttpContentEncoding> encodings)
        {
            _encodings = encodings;
        }

        public IEnumerable<IHttpContentEncoding> Encodings
        {
            get { return _encodings; }
        }

        public IHttpContentEncoding MatchFor(string acceptEncoding)
        {
            var acceptableValues = acceptEncoding
                .Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim());

            IHttpContentEncoding encoding = new PassthroughEncoding();
            foreach(var value in acceptableValues)
            {
                var matching = _encodings.FirstOrDefault(x => x.MatchingEncoding.Matches(value));
                if (matching == null) continue;
                
                encoding = matching;
                break;
            }

            return encoding;
        }

        public class PassthroughEncoding : IHttpContentEncoding
        {
            public ContentEncoding MatchingEncoding
            {
                get { throw new NotImplementedException(); }
            }

            public Stream Encode(Stream content)
            {
                return content;
            }
        }
    }
}