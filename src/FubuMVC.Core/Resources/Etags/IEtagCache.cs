using System.Collections.Generic;
using FubuMVC.Core.Http.Headers;

namespace FubuMVC.Core.Resources.Etags
{
    public interface IEtagCache
    {
        // Can be null
        string Current(string resourceHash);
        IEnumerable<Header> HeadersForEtag(string etag); 
        void Register(string resourceHash, string etag, IEnumerable<Header> cacheHeaders);
        void Eject(string resourceHash);
    }
}