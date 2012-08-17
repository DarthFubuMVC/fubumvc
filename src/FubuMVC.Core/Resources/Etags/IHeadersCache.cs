using System;
using System.Collections.Generic;
using FubuCore.Util;
using FubuMVC.Core.Http.Headers;
using System.Linq;

namespace FubuMVC.Core.Resources.Etags
{
    public interface IHeadersCache
    {
        IEnumerable<Header> Current(string hash);
        void Register(string hash, IEnumerable<Header> headers);
        void Eject(string hash);
    }

    public class HeadersCache : IHeadersCache
    {
        private readonly Cache<string, IEnumerable<Header>> _headers = new Cache<string, IEnumerable<Header>>();

        public IEnumerable<Header> Current(string hash)
        {
            if (_headers.Has(hash)) return _headers[hash];

            return new Header[0];
        }

        public void Register(string hash, IEnumerable<Header> headers)
        {
            _headers[hash] = headers.ToArray();
        }

        public void Eject(string hash)
        {
            _headers.Remove(hash);
        }
    }
}