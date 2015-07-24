using System;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Dates;

namespace FubuMVC.RavenDb.Storage
{
    public class SoftDeletedEntityStorage<T> : IEntityStorage<T> where T : class, ISoftDeletedEntity
    {
        private readonly IEntityStorage<T> _inner;
        private readonly ISystemTime _systemTime;

        public SoftDeletedEntityStorage(IEntityStorage<T> inner, ISystemTime systemTime)
        {
            _inner = inner;
            _systemTime = systemTime;
        }

        public IEntityStorage<T> Inner
        {
            get { return _inner; }
        }

        public T Find(Guid id)
        {
            return _inner.Find(id);
        }

        public void Update(T entity)
        {
            _inner.Update(entity);
        }

        public void Remove(T entity)
        {
            entity.Deleted = new Milestone(_systemTime.UtcNow());

            Update(entity);
        }

        public IQueryable<T> All()
        {
            return _inner.All().Where(x => x.Deleted == null);
        }

        public void DeleteAll()
        {
            _inner.DeleteAll();
        }

        public T FindSingle(Expression<Func<T, bool>> filter)
        {
            return _inner.FindSingle(filter);
        }
    }
}