using System;
using FubuCore.Util;
using FubuFastPack.Persistence;
using StructureMap;

namespace FubuFastPack.StructureMap
{
    public class StructureMapEntityFinderCache : IEntityFinderCache
    {
        private readonly IContainer _container;
        private readonly Cache<Type, object> _finders = new Cache<Type, object>();

        public StructureMapEntityFinderCache(IContainer container)
        {
            _container = container;
            _finders.OnMissing = type =>
            {
                var funcType = typeof (Func<,,>).MakeGenericType(typeof (IRepository), typeof (string), type);
                var func = _container.TryGetInstance(funcType);

                if (func == null)
                {
                    throw new ArgumentOutOfRangeException("No finder Func<IRepository, string, T> is registered for ", type.FullName);
                }

                return func;
            };
        }

        public Func<IRepository, string, T> FinderFor<T>()
        {
            return (Func<IRepository, string, T>) _finders[typeof(T)];
        }
    }
}