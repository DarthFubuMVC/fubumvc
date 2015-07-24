using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.RavenDb.Storage
{
    public class StorageFactory : IStorageFactory
    {
        private readonly IPersistor _persistor;
        private readonly IEnumerable<IEntityStoragePolicy> _policies;

        public StorageFactory(IPersistor persistor, IEnumerable<IEntityStoragePolicy> policies)
        { 
            _persistor = persistor;
            _policies = policies;
        }

        public IEntityStorage<T> StorageFor<T>() where T : class, IEntity
        {
            IEntityStorage<T> storage = new EntityStorage<T>(_persistor);
            _policies.Where(x => x.Matches<T>()).Each(policy => {
                storage = policy.Wrap(storage);
            });

            return storage;
        }
    }
}