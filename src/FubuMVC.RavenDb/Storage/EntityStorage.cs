using System;
using System.Linq;
using System.Linq.Expressions;

namespace FubuMVC.RavenDb.Storage
{
    public class EntityStorage<T> : IEntityStorage<T> where T : class, IEntity
    {
        private readonly IPersistor _persistor;

        public EntityStorage(IPersistor persistor)
        {
            _persistor = persistor;
        }

        public T Find(Guid id)
        {
            return _persistor.Find<T>(id);
        }

        public void Update(T entity)
        {
            _persistor.Persist(entity);
        }

        public void Remove(T entity)
        {
            _persistor.Remove(entity);
        }

        public IQueryable<T> All()
        {
            return _persistor.LoadAll<T>();
        }

        public void DeleteAll()
        {
            _persistor.DeleteAll<T>();
        }

        public T FindSingle(Expression<Func<T, bool>> filter)
        {
            return _persistor.FindSingle(filter);
        }
    }
}