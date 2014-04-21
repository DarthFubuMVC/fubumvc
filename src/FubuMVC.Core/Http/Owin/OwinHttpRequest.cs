using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml.Serialization;
using FubuCore;
using FubuMVC.Core.Http.Cookies;
using HtmlTags;

namespace FubuMVC.Core.Http.Owin
{
    // TODO -- OwinHttpRequest is too big.  Thin it down.  Extract some responsibilities
    public class OwinHttpRequest : IHttpRequest
    {
        public static OwinHttpRequest ForTesting()
        {
            var request = new OwinHttpRequest();
            request.HttpMethod("GET");
            request.FullUrl("http://server");


            return request;
        }

        private readonly IDictionary<string, object> _environment;
        private readonly Lazy<IDictionary<string, string[]>> _headers;
        private readonly Lazy<NameValueCollection> _querystring;

        public OwinHttpRequest() : this(new Dictionary<string, object>())
        {
            _environment.Add(OwinConstants.RequestHeadersKey, new Dictionary<string, string[]>());
            _querystring = new Lazy<NameValueCollection>(() => new NameValueCollection());
            _environment.Add(OwinConstants.ResponseBodyKey, new MemoryStream());
        }

        public OwinHttpRequest(IDictionary<string, object> environment)
        {
            _environment = environment;
            _headers =
                new Lazy<IDictionary<string, string[]>>(
                    () => { return environment.Get<IDictionary<string, string[]>>(OwinConstants.RequestHeadersKey); });

            _querystring =
                new Lazy<NameValueCollection>(
                    () => {
                        if (!environment.ContainsKey(OwinConstants.RequestQueryStringKey)) return new NameValueCollection();

                        var values = HttpUtility.ParseQueryString(environment.Get<string>(OwinConstants.RequestQueryStringKey));

                        return values;
                    });

            Cookies = new Cookies.Cookies(this);
        }

        public IDictionary<string, object> Environment
        {
            get { return _environment; }
        }

        private T get<T>(string key)
        {
            object value;
            return _environment.TryGetValue(key, out value) ? (T) value : default(T);
        }

        private void append(string key, object o)
        {
            if (_environment.ContainsKey(key))
            {
                _environment[key] = o;
            }
            else
            {
                _environment.Add(key, o);
            }
        }

        public string RawUrl()
        {
            var queryString = get<string>(OwinConstants.RequestQueryStringKey);
            if (queryString.IsEmpty())
            {
                return get<string>(OwinConstants.RequestPathBaseKey) + get<string>(OwinConstants.RequestPathKey);
            }

            return get<string>("owin.RequestPathBase") + get<string>("owin.RequestPath") + "?" +
                   get<string>("owin.RequestQueryString");
        }

        public string RelativeUrl()
        {
            return _environment.Get<string>(OwinConstants.RequestPathKey).TrimStart('/');
        }

        public void AppendCookie(Cookie cookie)
        {
            if (_headers.Value.ContainsKey(HttpRequestHeaders.Cookie))
            {
                _headers.Value[HttpRequestHeaders.Cookie][0] = _headers.Value[HttpRequestHeaders.Cookie][0] + "; " +
                                                            cookie.ToString();
            }
            else
            {
                Header(HttpRequestHeaders.Cookie, cookie.ToString());
            }

            
        }

        public OwinHttpRequest RelativeUrl(string url)
        {
            var parts = url.Split('?');

            append(OwinConstants.RequestPathKey, parts.First());

            if (url.Contains("?"))
            {
                var querystring = parts.Last();
                if (_environment.ContainsKey(OwinConstants.RequestQueryStringKey))
                {
                    _environment[OwinConstants.RequestQueryStringKey] = querystring;
                }
                else
                {
                    _environment.Add(OwinConstants.RequestQueryStringKey, querystring);
                }
            }

            return this;
        }

        public OwinHttpRequest FullUrl(string url)
        {
            var uri = new Uri(url);
            append(OwinConstants.RequestSchemeKey, uri.Scheme);
            append(OwinConstants.RequestPathBaseKey, string.Empty);
            Header(HttpRequestHeaders.Host, uri.Host);
            append(OwinConstants.RequestPathKey, uri.AbsolutePath);

            return this;
        }


        public string FullUrl()
        {
            var requestPath = get<string>(OwinConstants.RequestPathKey);


            var uriBuilder = uriBuilderFor(requestPath);


            var requestQueryString = get<string>(OwinConstants.RequestQueryStringKey);
            if (requestQueryString.IsNotEmpty())
            {
                uriBuilder.Query = requestQueryString;
            }

            return uriBuilder.Uri.ToString();
        }

        private UriBuilder uriBuilderFor(string requestPath)
        {
            var requestScheme = get<string>(OwinConstants.RequestSchemeKey);
            var requestPathBase = get<string>(OwinConstants.RequestPathBaseKey);

            // default values, in absence of a host header
            var host = "127.0.0.1";
            var port = String.Equals(requestScheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ? 443 : 80;

            // if a single host header is available
            string[] hostAndPort;
            if (_headers.Value.TryGetValue("Host", out hostAndPort) &&
                hostAndPort != null &&
                hostAndPort.Length == 1 &&
                !String.IsNullOrWhiteSpace(hostAndPort[0]))
            {
                var parts = hostAndPort[0].Split(':');
                host = parts[0];
                if (parts.Length > 1)
                {
                    int.TryParse(parts[1], out port);
                }
            }

            var uriBuilder = new UriBuilder(requestScheme, host, port, requestPathBase + requestPath);
            return uriBuilder;
        }

        public string ToFullUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute)) return url;

            if (url.StartsWith("~/"))
            {
                // TODO -- need to use the OwinRequestPathBase whatever in this case.  Not really important now, but might 
                // be down the road.
                return url.TrimStart('~');
            }

            if (!url.StartsWith("/"))
            {
                return "/" + url;
            }

            var requestScheme = get<string>(OwinConstants.RequestSchemeKey) + "://";
            if (url.StartsWith(requestScheme, StringComparison.OrdinalIgnoreCase)) return url;

            return uriBuilderFor(url).Uri.ToString();
        }

        public string HttpMethod()
        {
            return get<string>(OwinConstants.RequestMethodKey);
        }

        public OwinHttpRequest HttpMethod(string method)
        {
            append(OwinConstants.RequestMethodKey, method);
            return this;
        }

        public OwinHttpRequest Header(string key, params string[] values)
        {
            values.Each(x => _headers.Value.AppendValue(key, x));

            return this;
        }

        public OwinHttpRequest ContentType(string contentType)
        {
            return Header(HttpRequestHeaders.ContentType, contentType);
        }

        public OwinHttpRequest Accepts(string accepts)
        {
            return Header(HttpRequestHeaders.Accept, accepts);
        }

        public OwinHttpRequest IfNoneMatch(string etag)
        {
            return Header(HttpRequestHeaders.IfNoneMatch, etag);
        }

        public OwinHttpRequest IfMatch(string etag)
        {
            return Header(HttpRequestHeaders.IfMatch, etag);
        }

        public OwinHttpRequest IfModifiedSince(DateTime time)
        {
            return Header(HttpRequestHeaders.IfModifiedSince, time.ToUniversalTime().ToString("r"));
        }

        public OwinHttpRequest IfUnModifiedSince(DateTime time)
        {
            return Header(HttpRequestHeaders.IfUnmodifiedSince, time.ToUniversalTime().ToString("r"));
        }

        public bool HasHeader(string key)
        {
            return _headers.Value.ContainsKey(key) || _headers.Value.Keys.Any(x => x.EqualsIgnoreCase(key));
        }

        public IEnumerable<string> GetHeader(string key)
        {
            if (!HasHeader(key)) return new string[0];

            key = _headers.Value.Keys.FirstOrDefault(x => x.EqualsIgnoreCase(key));
            return key.IsEmpty() ? new string[0] : _headers.Value.Get(key);
        }

        public IEnumerable<string> AllHeaderKeys()
        {
            return _headers.Value.Keys;
        }

        public NameValueCollection QueryString
        {
            get { return _querystring.Value; }
        }

        public NameValueCollection Form
        {
            get
            {
                if (!_environment.ContainsKey(OwinConstants.RequestFormKey))
                {
                    _environment.Add(OwinConstants.RequestFormKey, new NameValueCollection());
                }

                return _environment.Get<NameValueCollection>(OwinConstants.RequestFormKey);
            }
        }


        public Stream Input
        {
            get
            {
                if (!_environment.ContainsKey(OwinConstants.RequestBodyKey))
                {
                    _environment.Add(OwinConstants.RequestBodyKey, new MemoryStream());
                }

                return _environment.Get<Stream>(OwinConstants.RequestBodyKey);
            }
        }

        public bool IsClientConnected()
        {
            var cancellation = _environment.Get<CancellationToken>(OwinConstants.CallCancelledKey);
            return cancellation == null ? false : !cancellation.IsCancellationRequested;
        }

        public ICookies Cookies { get; private set; }


        public HttpRequestBody Body
        {
            get { return new HttpRequestBody(this); }
        }

        public class HttpRequestBody
        {
            private readonly OwinHttpRequest _parent;

            public HttpRequestBody(OwinHttpRequest parent)
            {
                _parent = parent;
            }

            public void XmlInputIs(object target)
            {
                var serializer = new XmlSerializer(target.GetType());
                serializer.Serialize(_parent.Input, target);
                _parent.Input.Position = 0;
            }

            public void JsonInputIs(object target)
            {
                var json = JsonUtil.ToJson(target);

                JsonInputIs(json);
            }

            public void JsonInputIs(string json)
            {
                var writer = new StreamWriter(_parent.Input);
                writer.Write(json);
                writer.Flush();

                _parent.Input.Position = 0;
            }

            public void ReplaceBody(Stream stream)
            {
                stream.Position = 0;
                _parent.append(OwinConstants.RequestBodyKey, stream);
            }
        }

        public void RewindData()
        {
            if (_environment.ContainsKey(OwinConstants.RequestFormKey) && Form.Count > 0)
            {
                var post = formData().Join("&");
                var postBytes = Encoding.Default.GetBytes(post);
                Input.Write(postBytes, 0, postBytes.Length);

                _environment.Remove(OwinConstants.RequestFormKey);
            }

            if (_environment.ContainsKey(OwinConstants.RequestBodyKey))
            {
                Input.Position = 0;
            }
        }

        private IEnumerable<string> formData()
        {
            foreach (var key in Form.AllKeys)
            {
                yield return "{0}={1}".ToFormat(key, HttpUtility.HtmlEncode(Form[key]));
            }
        } 

    }
}