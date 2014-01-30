using System;
using System.Collections.Generic;
using System.Net;
using FubuCore;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using System.Linq;

namespace FubuMVC.OwinHost
{
    public class OwinResponse : IResponse
    {
        private readonly IDictionary<string, object> _environment;

        public OwinResponse(IDictionary<string, object> environment)
        {
            _environment = environment;
        }

        public int StatusCode
        {
            get
            {
                return _environment.Get<int>(OwinConstants.ResponseStatusCodeKey);
            }
        }

        public string StatusDescription
        {
            get
            {
                return _environment.Get<string>(OwinConstants.ResponseReasonPhraseKey);
            }
        }

        public string HeaderValueFor(HttpResponseHeader key)
        {
            return HeaderValueFor(HttpResponseHeaders.HeaderNameFor(key));
        }

        public string HeaderValueFor(string headerKey)
        {
            return _environment.Get<IDictionary<string, string[]>>(OwinConstants.ResponseHeadersKey).Get(headerKey).FirstOrDefault();
        }

        public IEnumerable<Header> AllHeaders()
        {
            return
                _environment.Get<IDictionary<string, string[]>>(OwinConstants.ResponseHeadersKey).SelectMany(pair => {
                    return pair.Value.Select(x => new Header(pair.Key, x));
                });
        }
    }
}