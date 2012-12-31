using System;
using System.Collections.Generic;
using System.Web;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Cookies;

namespace FubuMVC.OwinHost
{
    public class OwinCookies : ICookies
    {
        public OwinCookies(IDictionary<string, string[]> headers)
        {
            throw new NotImplementedException();
        }

        public bool Has(string name)
        {
            throw new System.NotImplementedException();
        }

        public HttpCookie Get(string name)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<HttpCookie> Request { get; private set; }
        public IEnumerable<HttpCookie> Response { get; private set; }
    }
}