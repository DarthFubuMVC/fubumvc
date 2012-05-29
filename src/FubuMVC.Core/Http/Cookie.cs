using System;
using System.Collections.Generic;
using System.Web;

namespace FubuMVC.Core.Http
{
    public interface ICookies
    {
        HttpCookieCollection Request { get; }
        HttpCookieCollection Response { get; }
    }

    public class InMemoryCookies : ICookies
    {
        public InMemoryCookies()
        {
            Request = new HttpCookieCollection();
            Response = new HttpCookieCollection();
        }

        public HttpCookieCollection Request
        {
            get; private set;
        }

        public HttpCookieCollection Response
        {
            get; private set;
        }
    }
}