using System;
using System.Collections.Generic;
using FubuCore.Util;

namespace FubuMVC.Core.Http.Headers
{
    public class HttpHeaderValues : IHaveHeaders
    {
        private readonly Cache<string, string> _headers = new Cache<string, string>();

        public HttpHeaderValues()
        {
        }

        public HttpHeaderValues(string key, string value)
        {
            this[key] = value;
        }

        public string this[string key]
        {
            get
            {
                return _headers[key];
            }
            set
            {
                _headers[key] = value;
            }
        }

        public bool Has(string name)
        {
            return _headers.Has(name);
        }


        public IEnumerable<Header> Headers
        {
            get
            {
                foreach (var key in _headers.GetAllKeys())
                {
                    yield return new Header(key, _headers[key]);
                }
            }
        }

        public static HttpHeaderValues ForETag(string etag)
        {
            return new HttpHeaderValues(HttpResponseHeaders.ETag, etag);
        }
    }
}