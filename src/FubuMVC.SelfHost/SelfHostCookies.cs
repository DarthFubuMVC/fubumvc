using System;
using System.Net.Http;
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

        public HttpCookieCollection Request
        {
            get { throw new NotImplementedException(); }
        }

        public HttpCookieCollection Response
        {
            get { throw new NotImplementedException(); }
        }
    }
}