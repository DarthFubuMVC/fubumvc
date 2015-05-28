using System;
using FubuCore.Dates;
using FubuCore;

namespace FubuPersistence.Storage
{
    public class SoftDeletedStoragePolicy : IEntityStoragePolicy
    {
        private readonly ISystemTime _systemTime;

        public SoftDeletedStoragePolicy(ISystemTime systemTime)
        {
            _systemTime = systemTime;
        }

        public bool Matches<T>() where T : class, IEntity
        {
            return typeof (T).CanBeCastTo<ISoftDeletedEntity>();
        }

        public IEntityStorage<T> Wrap<T>(IEntityStorage<T> inner) where T : class, IEntity
        {
            var storageType = typeof (SoftDeletedEntityStorage<>).MakeGenericType(typeof (T));
            return (IEntityStorage<T>) Activator.CreateInstance(storageType, inner, _systemTime);
        }
    }
}