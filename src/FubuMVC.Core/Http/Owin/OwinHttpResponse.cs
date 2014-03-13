using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core.Http.Compression;
using FubuMVC.Core.Http.Headers;

namespace FubuMVC.Core.Http.Owin
{
    public class OwinHttpResponse : IHttpResponse, IDisposable
    {
        private readonly IDictionary<string, object> _environment;
        private readonly Stream _output;

        public OwinHttpResponse() : this(new Dictionary<string, object>())
        {
        }

        public OwinHttpResponse(IDictionary<string, object> environment)
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
                var sendFile = _environment.Get<Func<string, long, long?, CancellationToken, Task>>("sendfile.SendAsync");
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

        public int StatusCode
        {
            get
            {
                return _environment.Get<int>(OwinConstants.ResponseStatusCodeKey);
            }
            set
            {
                _environment.Set<int>(OwinConstants.ResponseStatusCodeKey, value);
            }
        }

        public string StatusDescription
        {
            get
            {
                return _environment.Get<string>(OwinConstants.ResponseReasonPhraseKey);
            }
            set
            {
                _environment.Set<string>(OwinConstants.ResponseReasonPhraseKey, value);
            }
        }

        public IEnumerable<string> HeaderValueFor(string headerKey)
        {
            return _environment.Get<IDictionary<string, string[]>>(OwinConstants.ResponseHeadersKey).Get(headerKey);
        }

        public IEnumerable<Header> AllHeaders()
        {
            return
                _environment.Get<IDictionary<string, string[]>>(OwinConstants.ResponseHeadersKey).SelectMany(pair =>
                {
                    return pair.Value.Select(x => new Header(pair.Key, x));
                });
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

    public static class DictionaryExtensions
    {
        public static void Set<T>(this IDictionary<string, object> dict, string key, T value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }
    }
}