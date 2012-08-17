using System;

namespace FubuMVC.Core.Caching
{
    public interface IOutputCache
    {
        IRecordedOutput Retrieve(string resourceHash, Func<IRecordedOutput> cacheMiss);
        void Eject(string resourceHash);
        void FlushAll();
    }
}