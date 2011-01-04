using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuFastPack.Domain;
using FubuFastPack.NHibernate;
using FubuFastPack.Querying;
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
        private readonly IEnumerable<IDataRestriction> _dataRestrictions;

        public Repository(ISession session, IEntityFinder finder, IConfigurationSource source, IEnumerable<IDataRestriction> dataRestrictions)
        {
            if (session == null) throw new ArgumentNullException("session");

            _session = session;

            _finder = finder;
            _source = source;
            _dataRestrictions = dataRestrictions;
        }

        public DomainEntity FindByPath(string path)
        {
            var parts = path.Split('/');
            var typeName = parts[0];
            Guid id;
            if (!Guid.TryParse(parts[1], out id)) throw new ArgumentException("The value '{0}' does not contain a valid GUID identifier.", "path");

            var theType = _source.Configuration().PersistedTypeByName(typeName);
            if (theType == null) throw new ArgumentException("The value '{0}' refers to an unknown entity type.".ToFormat(path), "path");
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

        public IQueryable<T> RestrictedQuery<T>() where T : DomainEntity
        {
            var restrictions = _dataRestrictions.OfType<IDataRestriction<T>>();
            if (!restrictions.Any()) return Query<T>();

            var projection = new Projection<T>(_session);
            restrictions.Each(r => r.Apply(projection));
            var filteredCriteria = projection.GetFilteredCriteria();
            return _session.Linq<T>(filteredCriteria);
        }
    }
}