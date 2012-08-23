using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using FubuMVC.Core.Http.Headers;
using System.Linq;

namespace FubuMVC.Core.Http.AspNet
{
    public class AspNetResponse : IResponse
    {
        private readonly HttpResponseBase _response;

        public AspNetResponse(HttpResponseBase response)
        {
            _response = response;
        }

        public int StatusCode
        {
            get { return _response.StatusCode; }
        }

        public string StatusDescription
        {
            get { return _response.StatusDescription; }
        }

        public string HeaderValueFor(HttpResponseHeader key)
        {
            return HeaderValueFor(HttpResponseHeaders.HeaderNameFor(key));
        }

        public string HeaderValueFor(string headerKey)
        {
            return _response.Headers[headerKey];
        }

        public IEnumerable<Header> AllHeaders()
        {
            var keys = _response.Headers.AllKeys;
            return keys.Select(x => new Header(x, _response.Headers[x])).ToArray();
        }
    }
}