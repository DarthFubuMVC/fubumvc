using System;
using System.Collections.Generic;
using System.Net;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using System.Linq;

namespace FubuMVC.OwinHost
{
    public class OwinResponse : IResponse
    {
        private readonly Response _response;

        public OwinResponse(Response response)
        {
            _response = response;
        }

        public int StatusCode
        {
            get
            {
                var parts = _response.Status.Split(' ');

                return int.Parse(parts.First());
            }
        }

        public string StatusDescription
        {
            get
            {
                var parts = _response.Status.Split(' ');
                if (parts.Length == 1)
                {
                    return null;
                }

                return parts.Skip(1).Join(" ");
            }
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
            return _response.Headers.Select(x =>
            {
                return new Header(x.Key, x.Value);
            }).ToArray();
        }
    }
}