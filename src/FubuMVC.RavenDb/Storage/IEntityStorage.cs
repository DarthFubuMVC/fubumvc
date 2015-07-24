using System;
using System.Linq;
using System.Linq.Expressions;

namespace FubuMVC.RavenDb.Storage
{
    public interface IEntityStorage<T> where T : class, IEntity
    {
        T Find(Guid id);
        void Update(T entity);
        void Remove(T entity);

        IQueryable<T> All();
        void DeleteAll();
        T FindSingle(Expression<Func<T, bool>> filter);
    }
}