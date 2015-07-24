using System;
using System.Linq;
using System.Linq.Expressions;

namespace FubuMVC.RavenDb
{
    public interface IEntityRepository
    {
        IQueryable<T> All<T>() where T : class, IEntity;
        T Find<T>(Guid id) where T : class, IEntity;
        void Update<T>(T model) where T : class, IEntity;
        void Remove<T>(T model) where T : class, IEntity;

        T FindWhere<T>(Expression<Func<T, bool>> filter) where T : class, IEntity;

        /// <summary>
        ///   This should only be used in testing!!!!
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        void DeleteAll<T>() where T : class, IEntity;
    }
}