using System;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuFastPack.Domain;
using FubuFastPack.NHibernate;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace FubuFastPack.Persistence
{
    public class Repository : IRepository
    {
        private readonly ISession _session;
        private readonly IEntityFinder _finder;
        private readonly IConfigurationSource _source;

        public Repository(ISession session, IEntityFinder finder, IConfigurationSource source)
        {
            if (session == null) throw new ArgumentNullException("session");

            _session = session;

            _finder = finder;
            _source = source;
        }

        public DomainEntity FindByPath(string path)
        {
            var parts = path.Split('/');
            var typeName = parts[0];
            var id = new Guid(parts[1]);

            var theType = _source.Configuration().PersistedTypeByName(typeName);

            return _session.Get(theType, id) as DomainEntity;
        }

        public T Find<T>(Guid id) where T : Entity
        {
            return _session.Get<T>(id);
        }

        public T Find<T>(string text) where T : Entity
        {
            return text.IsEmpty() ? null : _finder.Find<T>(this, text);
        }

        public void Delete(object target)
        {
            _session.Delete(target);
        }

        public void RejectChanges(object target)
        {
            _session.Evict(target);
        }

        public void FlushChanges()
        {
            _session.Flush();
        }

        public IQueryable<T> Query<T>()
        {
            return _session.Linq<T>();
        }

        public IQueryable<T> Query<T>(Expression<Func<T, bool>> where)
        {
            return _session.Linq<T>().Where(where);
        }

        public IQueryable<T> Query<T>(IQueryExpression<T> queryExpression) where T : DomainEntity
        {
            return queryExpression.Apply(_session.Linq<T>());
        }

        public void Save(object target)
        {
            _session.SaveOrUpdate(target);
        }

        public void Insert(object target)
        {
            _session.Save(target);
        }

        public T[] GetAll<T>()
        {
            return _session.CreateCriteria(typeof(T)).List<T>().ToArray();
        }

        public T FindBy<T, U>(Expression<Func<T, U>> expression, U search) where T : class
        {
            var propertyName = ReflectionHelper.GetProperty(expression).Name;
            var criteria = _session.CreateCriteria(typeof(T)).Add(Restrictions.Eq(propertyName, search));
            return criteria.UniqueResult() as T;
        }

        public T FindBy<T>(Expression<Func<T, bool>> where)
        {
            return _session.Linq<T>().FirstOrDefault(where);
        }
    }
}