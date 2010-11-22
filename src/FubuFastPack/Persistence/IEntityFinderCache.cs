using System;

namespace FubuFastPack.Persistence
{
    public interface IEntityFinderCache
    {
        Func<IRepository, string, T> FinderFor<T>();
    }
}