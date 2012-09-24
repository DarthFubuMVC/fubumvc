using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;

namespace FubuMVC.SelfHost
{
    public class SelfHostResponse : IResponse
    {
        private readonly HttpResponseMessage _response;

        public SelfHostResponse(HttpResponseMessage response)
        {
            _response = response;
        }

        public string HeaderValueFor(HttpResponseHeader key)
        {
            throw new NotImplementedException();
        }

        public string HeaderValueFor(string headerKey)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Header> AllHeaders()
        {
            throw new NotImplementedException();
        }

        public int StatusCode
        {
            get { throw new NotImplementedException(); }
        }

        public string StatusDescription
        {
            get { throw new NotImplementedException(); }
        }
    }
}