using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.Http.Cookies
{
    public static class HttpCookieFormatter
    {
        private const string ExpiresToken = "expires";
        private const string MaxAgeToken = "max-age";
        private const string DomainToken = "domain";
        private const string PathToken = "path";
        private const string SecureToken = "secure";
        private const string HttpOnlyToken = "httponly";

        private static readonly Cache<string, Action<Cookie, string>> _setters =
            new Cache<string, Action<Cookie, string>>();


        static HttpCookieFormatter()
        {
            _setters[ExpiresToken] = (cookie, value) => { throw new NotImplementedException(); };

            _setters[MaxAgeToken] = (cookie, value) => {
                try
                {
                    var seconds = int.Parse(value.Trim());
                    cookie.MaxAge = seconds.Seconds();
                }
                catch (Exception)
                {
                    
                    // doing nothing here
                }
            };

            _setters[DomainToken] = (cookie, value) => { cookie.Domain = value; };

            _setters[PathToken] = (cookie, value) => {
                cookie.Path = value.IsEmpty() ? "/" : value;
            };

            _setters[SecureToken] = (cookie, value) => { cookie.Secure = true; };

            _setters[HttpOnlyToken] = (cookie, value) => { cookie.HttpOnly = true; };

        }

        public static Cookie ToCookie(string headerValue)
        {
            if (headerValue.IsEmpty()) return null;

            var cookie = new Cookie();

            var segments = headerValue.TrimEnd().TrimEnd(';').Split(';').Select(x => new Segment(x.Trim()));
            segments.Each(segment => {
                string canonicalKey = segment.Key.ToLowerInvariant();

                if (_setters.Has(canonicalKey))
                {
                    _setters[canonicalKey](cookie, segment.Value);
                }
                else
                {
                    var state = CookieState.For(segment);
                    cookie.Add(state);
                }
            });

            return cookie;
        }

        public static string WriteHeaderValue(HttpCookie cookie)
        {
            throw new NotImplementedException();
        }


    }
}