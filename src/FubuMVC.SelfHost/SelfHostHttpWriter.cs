using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using FubuCore;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Compression;
using HttpResponseHeaders = FubuMVC.Core.Http.HttpResponseHeaders;

namespace FubuMVC.SelfHost
{
    public class SelfHostHttpWriter : IHttpWriter
    {
        private static readonly IList<string> _contentNames = new List<string>();

        static SelfHostHttpWriter()
        {
            _contentNames.Add(HttpResponseHeaders.ContentMd5);
            _contentNames.Add(HttpResponseHeaders.ContentDisposition);
            _contentNames.Add(HttpResponseHeaders.ContentLocation);
            _contentNames.Add(HttpResponseHeaders.Allow);
            _contentNames.Add(HttpResponseHeaders.ContentEncoding);
            _contentNames.Add(HttpResponseHeaders.ContentLength);
            _contentNames.Add(HttpResponseHeaders.ContentLanguage);
            _contentNames.Add(HttpResponseHeaders.ContentRange);
            _contentNames.Add(HttpResponseHeaders.Expires);
            _contentNames.Add(HttpResponseHeaders.LastModified);
        }

        private readonly HttpResponseMessage _response;
        private readonly SelfHostCurrentHttpRequest _httpRequest;
        private readonly Stream _output = new MemoryStream();
        private readonly Lazy<StreamWriter> _writer;
        private readonly IList<Action<StreamContent>> _modifications = new List<Action<StreamContent>>();
        private IHttpContentEncoding _encoding;

        public SelfHostHttpWriter(HttpResponseMessage response, SelfHostCurrentHttpRequest httpRequest)
        {
            _response = response;
            _httpRequest = httpRequest;
            _writer = new Lazy<StreamWriter>(() => new StreamWriter(_output));
        }

        private Action<StreamContent> write
        {
            set
            {
                _modifications.Add(value);
            }
        }

        public void AppendHeader(string key, string value)
        {
            if (_contentNames.Contains(key))
            {
                write = c => c.Headers.TryAddWithoutValidation(key, value);   
            }
            else
            {
                _response.Headers.TryAddWithoutValidation(key, value);
            }
        }

        public void WriteFile(string file)
        {
            using (var fileStream = new FileStream(file, FileMode.Open))
            {
                Write(stream => fileStream.CopyTo(stream, 64000));
            }
        }

        public void WriteContentType(string contentType)
        {
            write = c => c.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        }

        public void Write(string content)
        {
            _writer.Value.Write(content);
            _writer.Value.Flush();
        }

        public void Redirect(string url)
        {
            url = _httpRequest.ToFullUrl(url);
            WriteResponseCode(HttpStatusCode.Redirect, "Redirect");
            AppendHeader(HttpResponseHeaders.Location, url);

            var html = string.Format("<html><head><title>302 Found</title></head><body><h1>Found</h1><p>The document has moved <a href='{0}'>here</a>.</p></body></html>", url);
            Write(html);
        }

        public void WriteResponseCode(HttpStatusCode status, string description)
        {
            _response.StatusCode = status;
            _response.ReasonPhrase = description;
        }

        public void UseEncoding(IHttpContentEncoding encoding)
        {
            _encoding = encoding;
        }

        public void Write(Action<Stream> output)
        {
            output(_output);
        }

        public void Flush()
        {
            // no-op;
        }

        public void AttachContent()
        {
            _output.Position = 0;

            var streamContent = buildContent();

            _modifications.Each(x => x(streamContent));

            _response.Content = streamContent;
        }

        private StreamContent buildContent()
        {
            if (_encoding == null || _encoding is HttpContentEncoders.PassthroughEncoding)
            {
                return new StreamContent(_output);
            }

            return new CompressedContent(_output, _encoding);
        }
    }


    public class CompressedContent : StreamContent
    {
        private readonly IHttpContentEncoding _encoding;
        private StreamContent _original;

        public CompressedContent(Stream stream, IHttpContentEncoding encoding) : base(stream)
        {
            _encoding = encoding;
            _original = new StreamContent(stream);
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            var compressedStream = _encoding.Encode(stream);

            return _original.CopyToAsync(compressedStream).ContinueWith(tsk =>
            {
                if (compressedStream != null)
                {
                    compressedStream.Dispose();
                }
            });
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;

            return false;
        }
    }


}