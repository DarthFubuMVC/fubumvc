using System.Collections.Generic;

namespace FubuMVC.Core.Runtime
{
    public interface IResponseCaching
    {
        void CacheRequestAgainstFileChanges(IEnumerable<string> localFiles);
        void CacheRequestAgainstFileChanges(string file);
    }
}