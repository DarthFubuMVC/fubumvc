using System;
using FubuCore.Util;

namespace FubuMVC.Core.Resources.Etags
{
    public class EtagCache : IEtagCache
    {
        private readonly Cache<string, string> _etags = new Cache<string, string>(key => null);

        public string Current(string resourceHash)
        {
            return _etags[resourceHash];
        }

        public void Register(string resourceHash, string etag)
        {
            _etags[resourceHash] = etag;
        }

        public void Eject(string resourceHash)
        {
            _etags[resourceHash] = null;
        }
    }
}