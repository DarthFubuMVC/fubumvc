using System;
using System.Collections.Generic;
using System.Net;
using FubuCore.Util;
using FubuMVC.Core.Http.Headers;

namespace FubuMVC.Core.Resources.Etags
{
    public class EtagCache : IEtagCache
    {
        private readonly Cache<string, string> _etags = new Cache<string, string>(key => null);
        private readonly Cache<string,IEnumerable<Header>> _headersByEtag = new Cache<string, IEnumerable<Header>>(x => new[]{new Header(HttpResponseHeader.ETag, x) });

        public string Current(string resourceHash)
        {
            return _etags[resourceHash];
        }

        public IEnumerable<Header> HeadersForEtag(string etag)
        {
            return _headersByEtag[etag];
        }

        public void Register(string resourceHash, string etag, IEnumerable<Header> cacheHeaders)
        {
            _etags[resourceHash] = etag;
            _headersByEtag[etag] = cacheHeaders;
        }

        public void Eject(string resourceHash)
        {
            _etags[resourceHash] = null;
        }
    }
}