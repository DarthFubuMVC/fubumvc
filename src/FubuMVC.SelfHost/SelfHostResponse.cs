using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using System.Linq;

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
            return HeaderValueFor(HttpResponseHeaders.HeaderNameFor(key));
        }

        public string HeaderValueFor(string headerKey)
        {
            return headerValuesFor(headerKey).FirstOrDefault();
        }

        private IEnumerable<string> headerValuesFor(string headerKey)
        {
            var responseHeaders = _response.Headers.GetValues(headerKey);
            return _response.Content == null
                       ? responseHeaders
                       : responseHeaders.Union(_response.Content.Headers.GetValues(headerKey));
        }

        public IEnumerable<Header> AllHeaders()
        {
            foreach (var httpResponseHeader in _response.Headers)
            {
                foreach (var value in httpResponseHeader.Value)
                {
                    yield return new Header(httpResponseHeader.Key, value);
                }
            }

            if (_response.Content != null)
            {
                foreach (var httpResponseHeader in _response.Content.Headers)
                {
                    foreach (var value in httpResponseHeader.Value)
                    {
                        yield return new Header(httpResponseHeader.Key, value);
                    }
                }
            }
        }

        public int StatusCode
        {
            get { return (int) _response.StatusCode; }
        }

        public string StatusDescription
        {
            // TODO -- figure out how to do this?
            get { return _response.ReasonPhrase; }
        }
    }
}