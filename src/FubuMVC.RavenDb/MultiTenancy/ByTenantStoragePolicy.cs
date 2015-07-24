using System;
using FubuCore;
using FubuMVC.RavenDb.Storage;

namespace FubuMVC.RavenDb.MultiTenancy
{
    public class ByTenantStoragePolicy : IEntityStoragePolicy
    {
        private readonly ITenantContext _context;

        public ByTenantStoragePolicy(ITenantContext context)
        {
            _context = context;
        }

        public bool Matches<T>() where T : class, IEntity
        {
            return typeof (T).CanBeCastTo<ITenantedEntity>();
        }

        public IEntityStorage<T> Wrap<T>(IEntityStorage<T> inner) where T : class, IEntity
        {
            var storageType = typeof (ByTenantEntityStorage<>).MakeGenericType(typeof (T));
            return (IEntityStorage<T>) Activator.CreateInstance(storageType, inner, _context);
        }
    }
}