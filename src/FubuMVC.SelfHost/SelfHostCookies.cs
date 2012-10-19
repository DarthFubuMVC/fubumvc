using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            throw new NotImplementedException();
        }

        public HttpCookie Get(string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<HttpCookie> Request
        {
            get { return CookiesFor(_request.Headers.GetCookies()); }
        }

        public IEnumerable<HttpCookie> Response
        {
            get { yield break; }
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
                cookie.Value = state.Value;
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