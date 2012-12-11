using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using FubuMVC.Core.Http;

namespace FubuMVC.SelfHost
{
    public class SelfHostCookies : ICookies
    {
        private readonly HttpRequestMessage _request;
        private readonly HttpResponseMessage _response;

        public SelfHostCookies(HttpRequestMessage request, HttpResponseMessage response)
        {
            _request = request;
            _response = response;
        }

        public bool Has(string name)
        {
            // TODO -- optimize this
            return Get(name) != null;
        }

        public HttpCookie Get(string name)
        {
            // TODO -- optimize this
            return CookiesFor(_request.Headers.GetCookies())
                .FirstOrDefault(x => x.Name == name);
        }

        public IEnumerable<HttpCookie> Request
        {
            get { return CookiesFor(_request.Headers.GetCookies()); }
        }

        public IEnumerable<HttpCookie> Response
        {
            get { return CookiesFor(responseCookies()); }
        }

        private IEnumerable<CookieHeaderValue> responseCookies()
        {
            var result = new List<CookieHeaderValue>();
            IEnumerable<string> cookieHeaders;
            if (_response.Headers.TryGetValues("Cookie", out cookieHeaders))
            {
                cookieHeaders.Each(header =>
                {
                    CookieHeaderValue cookieHeaderValue;
                    if (CookieHeaderValue.TryParse(header, out cookieHeaderValue))
                    {
                        result.Add(cookieHeaderValue);
                    }
                });
            }

            return result;
        }

        // Keep these public for testing
        public static IEnumerable<HttpCookie> CookiesFor(IEnumerable<CookieHeaderValue> cookies)
        {
            return cookies.SelectMany(CookiesFor);
        }

        public static IEnumerable<HttpCookie> CookiesFor(CookieHeaderValue value)
        {
            return value.Cookies.Select(x =>
            {
                var cookie = new HttpCookie(x.Name)
                {
                    Path = value.Path,
                    Domain = value.Domain,
                    HttpOnly = value.HttpOnly,
                    Secure = value.Secure
                };

                if (value.Expires.HasValue)
                {
                    cookie.Expires = value.Expires.Value.UtcDateTime;
                }

                if (x.Values.Count == 1)
                {
                    cookie.Value = HttpUtility.UrlDecode(x.Values.ToString());
                    return cookie;
                }

                x.Values
                    .AllKeys
                    .Each(key =>
                          {
                              cookie.Values[key] = HttpUtility.UrlDecode(x.Values[key]);
                          });

                return cookie;
            });

        }
    }
}