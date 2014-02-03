using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using FubuCore;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Compression;
using FubuMVC.Core;

namespace FubuMVC.OwinHost
{
    using SendFileFunc = Func<string, long, long?, CancellationToken, Task>;

    public class OwinHttpWriter : IHttpWriter, IDisposable
    {
        private readonly IDictionary<string, object> _environment;
        private readonly Stream _output;

        public OwinHttpWriter(IDictionary<string, object> environment)
        {
            _environment = environment;

            _output = new MemoryStream();
        }

        public void AppendHeader(string key, string value)
        {
            if (!_environment.ContainsKey(OwinConstants.ResponseHeadersKey))
            {
                _environment.Add(OwinConstants.ResponseHeadersKey, new Dictionary<string, string[]>());
            }

            var headers = _environment.Get<IDictionary<string, string[]>>(OwinConstants.ResponseHeadersKey);
            headers.AppendValue(key, value);
        }

        public void WriteFile(string file)
        {
            var fileInfo = new FileInfo(file);

            if (_environment.ContainsKey("sendfile.SendAsync"))
            {
                var sendFile = _environment.Get<SendFileFunc>("sendfile.SendAsync");
                sendFile(file, 0, fileInfo.Length, _environment.Get<CancellationToken>(OwinConstants.CallCancelledKey));
            }
            else
            {
                AppendHeader(HttpResponseHeaders.ContentLength, fileInfo.Length.ToString(CultureInfo.InvariantCulture));
                using (var fileStream = new FileStream(file, FileMode.Open))
                {
                    Write(stream => fileStream.CopyTo(stream));
                }
            }

            
            
        }

        public void WriteContentType(string contentType)
        {
            AppendHeader(HttpResponseHeaders.ContentType, contentType);
        }

        public long ContentLength
        {
            get
            {
                var headers = _environment.Get<IDictionary<string, string[]>>(OwinConstants.ResponseHeadersKey);
                return headers.ContainsKey(OwinConstants.ContentLengthHeader)
                    ? long.Parse(headers[OwinConstants.ContentLengthHeader][0])
                    : 0;
            }
            set
            {
                var headers = _environment.Get<IDictionary<string, string[]>>(OwinConstants.ResponseHeadersKey);
                if (headers.ContainsKey(OwinConstants.ContentLengthHeader))
                {
                    headers[OwinConstants.ContentLengthHeader][0] = value.ToString();
                }
                else
                {
                    headers.Add(OwinConstants.ContentLengthHeader, new []{value.ToString()});
                }
            }
        }

        public void Write(string content)
        {
            var writer = new StreamWriter(_output){AutoFlush = true};

            writer.Write(content);
        }

        public void Redirect(string url)
        {
            if (url.StartsWith("~"))
            {
                url = url.TrimStart('~');
            }

            // TODO: This is a hack, better way to accomplish this?
            _environment[OwinConstants.ResponseStatusCodeKey] = HttpStatusCode.Redirect;
            AppendHeader("Location", url);
            Write(
                string.Format(
                    "<html><head><title>302 Found</title></head><body><h1>Found</h1><p>The document has moved <a href='{0}'>here</a>.</p></body></html>",
                    url));
        }

        public void WriteResponseCode(HttpStatusCode status, string description = null)
        {
            _environment[OwinConstants.ResponseStatusCodeKey] = status.As<int>();
            _environment[OwinConstants.ResponseReasonPhraseKey] = description;
        }

        public void UseEncoding(IHttpContentEncoding encoding)
        {
            // TODO -- Come back to this one. The integration tests can't be done until we do
        }

        public void Write(Action<Stream> output)
        {
            output(_output);
        }

        public void Flush()
        {
            if (_output.Length <= 0) return;

            _output.Position = 0;

            var owinOutput = _environment.Get<Stream>(OwinConstants.ResponseBodyKey);
            _output.CopyTo(owinOutput);

            owinOutput.Flush();
        }

        public void Dispose()
        {
            Flush();
        }
    }
}