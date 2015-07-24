using System;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Util;
using FubuMVC.RavenDb.Storage;

namespace FubuMVC.RavenDb
{
    public class EntityRepository : IEntityRepository
    {
        private readonly IStorageFactory _storageFactory;
        private readonly Cache<Type, object> _storageProviders = new Cache<Type, object>();

        public EntityRepository(IStorageFactory storageFactory)
        {
            _storageFactory = storageFactory;
        }

        public IQueryable<T> All<T>() where T : class, IEntity
        {
            return storage<T>().All();
        }


        public T FindWhere<T>(Expression<Func<T, bool>> filter) where T : class, IEntity
        {
            var raw = storage<T>().FindSingle(filter);
            return raw == null ? null : Find<T>(raw.Id);
        }

        public T Find<T>(Guid id) where T : class, IEntity
        {
            return storage<T>().Find(id);
        }

        // virtual for testing
        public void Update<T>(T model) where T : class, IEntity
        {
            if (model.Id == default(Guid) || model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
            }

            storage<T>().Update(model);
        }

        public void Remove<T>(T model) where T : class, IEntity
        {
            storage<T>().Remove(model);
        }

        public void DeleteAll<T>() where T : class, IEntity
        {
            storage<T>().DeleteAll();
        }

        private IEntityStorage<T> storage<T>() where T : class, IEntity
        {
            _storageProviders.Fill(typeof(T), t => _storageFactory.StorageFor<T>());

            return (IEntityStorage<T>)_storageProviders[typeof(T)];
        }

        public static EntityRepository InMemory()
        {
            return InMemory(x => { });
        }

        public static EntityRepository InMemory(Action<EntityRepositoryExpression> configure)
        {
            var expression = new EntityRepositoryExpression();
            configure(expression);

            return expression.Build();
        }
    }
}