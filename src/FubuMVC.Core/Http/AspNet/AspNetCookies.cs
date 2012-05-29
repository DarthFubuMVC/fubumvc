using System;
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

        public HttpCookieCollection Request
        {
            get { return _request; }
        }

        public HttpCookieCollection Response
        {
            get { return _response; }
        }
    }

}