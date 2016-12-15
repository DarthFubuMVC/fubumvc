using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using FubuCore;
using FubuMVC.Core.Http.Compression;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Http.Scenarios;
using FubuMVC.Core.Json;
using FubuMVC.Core.Runtime;
using HtmlTags;
using Cookie = FubuMVC.Core.Http.Cookies.Cookie;

namespace FubuMVC.Core.Http.Owin
{
    public class OwinHttpResponse : IHttpResponse, IDisposable
    {
        private readonly OwinHeaderSettings _headerSettings;
        private readonly IDictionary<string, object> _environment;
        private MemoryStream _output;

        public OwinHttpResponse() : this(new Dictionary<string, object>())
        {
        }

        public OwinHttpResponse(IDictionary<string, object> environment)
        {
            _environment = environment;

            _output = new MemoryStream();

            if (!environment.ContainsKey(OwinConstants.ResponseStatusCodeKey))
            {
                StatusCode = 200;
            }

            _headerSettings = environment.ContainsKey(OwinConstants.HeaderSettings)
                ? environment.Get<OwinHeaderSettings>(OwinConstants.HeaderSettings)
                : new OwinHeaderSettings();
        }

        public void AppendHeader(string key, string value)
        {
            if (!_environment.ContainsKey(OwinConstants.ResponseHeadersKey))
            {
                _environment.Add(OwinConstants.ResponseHeadersKey, new Dictionary<string, string[]>());
            }

            var headers = _environment.ResponseHeaders();
            if (!_headerSettings.AllowMultiple(key))
            {
                headers.Remove(key);
                headers.Add(key, new[] { value });
                return;
            }

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
                using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // TODO -- SMELLY AS ALL HELL
                    Write(stream => fileStream.CopyToAsync(stream)).GetAwaiter().GetResult();
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

        public Task Write(string content)
        {
            var writer = new StreamWriter(_output){AutoFlush = true};

            return writer.WriteAsync(content);
        }

        public void Redirect(string url)
        {
            if (url.StartsWith("~"))
            {
                url = url.TrimStart('~');
            }

            _environment[OwinConstants.ResponseStatusCodeKey] = HttpStatusCode.Redirect;
            AppendHeader("Location", url);
            Write(
                $"<html><head><title>302 Found</title></head><body><h1>Found</h1><p>The document has moved <a href='{url}'>here</a>.</p></body></html>");
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
                return _environment.ContainsKey(OwinConstants.ResponseReasonPhraseKey) ? _environment.Get<string>(OwinConstants.ResponseReasonPhraseKey) : string.Empty;
            }
            set
            {
                _environment.Set<string>(OwinConstants.ResponseReasonPhraseKey, value);
            }
        }

        public IEnumerable<string> HeaderValueFor(string headerKey)
        {
            if (!_environment.ContainsKey(OwinConstants.ResponseHeadersKey))
            {
                return new string[0];
            }

            return _environment.Get<IDictionary<string, string[]>>(OwinConstants.ResponseHeadersKey).Get(headerKey) ?? new string[0];
        }

        public IEnumerable<Header> AllHeaders()
        {
            if (!_environment.ContainsKey(OwinConstants.ResponseHeadersKey))
            {
                return Enumerable.Empty<Header>();
            }

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

        public Task Write(Func<Stream, Task> output)
        {
            return output(_output);
        }

        public void Flush()
        {
            if (_output.Length <= 0) return;

            StreamContents(_output);

            _output = new MemoryStream();
        }

        public void StreamContents(MemoryStream recordedStream)
        {
            recordedStream.Position = 0;

            var owinOutput = _environment.Get<Stream>(OwinConstants.ResponseBodyKey);
            recordedStream.CopyTo(owinOutput);

            recordedStream.Flush();
        }

        public Stream Output
        {
            get { return _output; }
        }

        public void Dispose()
        {
            Flush();
        }

        public DateTime? LastModified()
        {
            var lastModifiedString = HeaderValueFor(HttpResponseHeaders.LastModified).SingleOrDefault();
            return lastModifiedString.IsEmpty() ? (DateTime?)null : DateTime.ParseExact(lastModifiedString, "r", null);
        }

        public HttpResponseBody Body
        {
            get
            {
                return new HttpResponseBody(_environment.Get<Stream>(OwinConstants.ResponseBodyKey), _environment);
            }
        }

        public IEnumerable<Cookie> Cookies()
        {
            return HeaderValueFor(HttpResponseHeaders.SetCookie).Select(CookieParser.ToCookie);
        }

        public Cookie CookieFor(string name)
        {
            return Cookies().FirstOrDefault(x => x.Matches(name));
        }

        public string ContentType()
        {
            return HeaderValueFor(HttpResponseHeaders.ContentType).FirstOrDefault();
        }

        public bool ContentTypeMatches(MimeType mimeType)
        {
            return HeaderValueFor(HttpResponseHeaders.ContentType).Any(x => x.EqualsIgnoreCase(mimeType.Value));
        }


    }

    public class HttpResponseBody
    {
        private readonly Stream _stream;
        private readonly IDictionary<string, object> _environment;

        public HttpResponseBody(Stream stream, IDictionary<string, object> environment)
        {
            _stream = stream;
            _environment = environment;
        }

        public string ReadAsText()
        {
            return Read(s => s.ReadAllText());
        }

        public T Read<T>(Func<Stream, T> read)
        {
            _stream.Position = 0;
            return read(_stream);
        }

        public XmlDocument ReadAsXml()
        {
            Func<Stream, XmlDocument> read = s => {
                var body = s.ReadAllText();

                if (body.Contains("Error")) return null;

                var document = new XmlDocument();
                document.LoadXml(body);

                return document;
            };

            return Read(read);
        }

        public T ReadAsXml<T>() where T : class
        {
            _stream.Position = 0;
            var serializer = new XmlSerializer(typeof (T));
            return serializer.Deserialize(new XmlTextReader(new StringReader(_stream.ReadAllText()))) as T;
        }

        public T ReadAsJson<T>()
        {
            var json = ReadAsText();

            if (_environment.ContainsKey("scenario-support"))
            {
                return
                    _environment.Get<IScenarioSupport>("scenario-support")
                        .Get<IJsonSerializer>()
                        .Deserialize<T>(json);
            }

            
            return JsonUtil.Get<T>(json);
        }
    }
}
