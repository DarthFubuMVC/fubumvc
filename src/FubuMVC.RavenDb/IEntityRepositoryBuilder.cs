using System;
using System.Collections.Generic;
using FubuCore.Dates;
using FubuMVC.RavenDb.InMemory;
using FubuMVC.RavenDb.Storage;

namespace FubuMVC.RavenDb
{
    public class EntityRepositoryExpression
    {
        private readonly IList<IEntityStoragePolicy> _policies = new List<IEntityStoragePolicy>();
        private Func<EntityRepositoryExpression, EntityRepository> _builder;
        private ISystemTime _clock;

        public EntityRepositoryExpression()
        {
            UseSystemTime(SystemTime.Default());
            BuildWith(basicRepository);
        }

        private static EntityRepository basicRepository(EntityRepositoryExpression expression)
        {
            var storage = new StorageFactory(new InMemoryPersistor(), expression.Policies);
            return new EntityRepository(storage);
        }

        public void UseSystemTime(ISystemTime clock)
        {
            _clock = clock;
        }

        public void BuildWith(Func<EntityRepositoryExpression, EntityRepository> builder)
        {
            _builder = builder;
        }

        public void AddStoragePolicy<T>() where T : IEntityStoragePolicy, new()
        {
            AddStoragePolicy(new T());
        }

        public void AddStoragePolicy(IEntityStoragePolicy policy)
        {
            _policies.Add(policy);
        }

        public EntityRepository Build()
        {
            return _builder(this);
        }

        public IEnumerable<IEntityStoragePolicy> Policies { get { return _policies; } }
        public ISystemTime Clock { get { return _clock; } }
    }
}