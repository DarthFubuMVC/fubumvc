using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FubuMVC.Core.Http.AspNet
{
    public class AspNetCookies : ICookies
    {
        private readonly HttpCookieCollection _request;
        private readonly HttpCookieCollection _response;

        public AspNetCookies(HttpContextBase context)
        {
            _request = context.Request.Cookies;
            _response = context.Response.Cookies;
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
            get { return cookiesFor(_request); }
        }

        public IEnumerable<HttpCookie> Response
        {
            get { return cookiesFor(_response); }
        }

        private static IEnumerable<HttpCookie> cookiesFor(HttpCookieCollection collection)
        {
            return collection.AllKeys.Select(x => collection[x]);
        }
    }

}