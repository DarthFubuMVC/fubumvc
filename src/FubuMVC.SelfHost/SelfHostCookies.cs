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
            return cookies.Select(CookieFor);
        }

        public static HttpCookie CookieFor(CookieHeaderValue value)
        {
            var name = DetermineName(value);
            var cookie = new HttpCookie(name)
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

            FillValues(value, cookie);

            return cookie;
        }

        public static string DetermineName(CookieHeaderValue value)
        {
            var state = value.Cookies.SingleOrDefault();
            if(state == null) throw new ArgumentException("Must contain at least one CookieState", "value");

            return state.Name;
        }

        public static void FillValues(CookieHeaderValue value, HttpCookie cookie)
        {
            if (!value.Cookies.Any()) throw new ArgumentException("Must contain at least one CookieState", "value");

            var state = value.Cookies.Single();
            var values = state.Values;

            if(values.Count == 1)
            {
                cookie.Value = state.Values.ToString();
                return;
            }

            values
                .AllKeys
                .Each(x =>
                      {
                          cookie.Values[x] = values[x];
                      });
        }
    }
}