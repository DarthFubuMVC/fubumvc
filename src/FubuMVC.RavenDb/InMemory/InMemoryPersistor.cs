using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;

namespace FubuMVC.RavenDb.InMemory
{
    public class InMemoryPersistor : IPersistor
    {
        private readonly IList<IEntity> _objects = new List<IEntity>();


        public IQueryable<T> LoadAll<T>() where T : IEntity
        {
            return _objects.OfType<T>().AsQueryable();
        }

        public void Persist<T>(T subject) where T : class, IEntity
        {
            if (subject.Id == Guid.Empty)
            {
                subject.Id = Guid.NewGuid();
            }

            var old = _objects.OfType<T>().FirstOrDefault(x => x.Id == subject.Id);
            old.IfNotNull(x => _objects.Remove(x));

            _objects.Add(subject);
        }

        public void DeleteAll<T>() where T : IEntity
        {
            _objects.RemoveAll(x => x is T);
        }

        public void Remove<T>(T target) where T : IEntity
        {
            _objects.RemoveAll(x => x is T && x.Id == target.Id);
        }

        public T FindBy<T>(Expression<Func<T, bool>> filter) where T : class, IEntity
        {
            return _objects.OfType<T>().FirstOrDefault(filter.Compile());
        }

        public T Find<T>(Guid id) where T : class, IEntity
        {
            return FindBy<T>(x => x.Id == id);
        }

        public T FindSingle<T>(Expression<Func<T, bool>> filter) where T : class, IEntity
        {
            return LoadAll<T>().FirstOrDefault(filter);
        }

        public void WipeAndReplace(IEnumerable<IEntity> entities)
        {
            _objects.Clear();
            _objects.AddRange(entities);
        }
    }
}