using System;
using System.Linq;
using System.Linq.Expressions;

namespace FubuMVC.RavenDb
{
    public interface IPersistor
    {
        IQueryable<T> LoadAll<T>() where T : IEntity;
        void Persist<T>(T subject) where T : class, IEntity;
        void DeleteAll<T>() where T : IEntity;
        void Remove<T>(T target) where T : IEntity;

        T Find<T>(Guid id) where T : class, IEntity;
        T FindSingle<T>(Expression<Func<T, bool>> filter) where T : class, IEntity;
    }
}