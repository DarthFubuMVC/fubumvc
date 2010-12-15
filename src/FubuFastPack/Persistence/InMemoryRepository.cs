using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuFastPack.Domain;
using FubuCore;

namespace FubuFastPack.Persistence
{
    public class InMemoryRepository : IRepository
    {
        private static readonly object _lockObject = new object();
        private readonly IEntityFinder _finder;
        private readonly IList<object> _items = new List<object>();
        private readonly IList<Action<object>> _onSaveActions = new List<Action<object>>();

        private readonly List<object> _deletedItems = new List<object>();
        private readonly List<object> _revertedItems = new List<object>();
        private readonly IList<object> _savedItems = new List<object>();

        //private readonly EntityFinder _finder;

        private Guid _nextId = Guid.Empty;

        public InMemoryRepository(IEntityFinder finder)
        {
            _finder = finder;
        }

        public Guid LastIdAssigned
        {
            get { return _nextId; }
        }

        public IList<object> SavedItems
        {
            get { return _savedItems; }
        }

        public IList DeletedItems
        {
            get { return _deletedItems; }
        }

        public IList RevertedItems
        {
            get { return _revertedItems; }
        }

        public DomainEntity FindByPath(string path)
        {
            var parts = path.Split('/');
            if (parts.Length != 2) return null;

            var typeName = parts[0];


            var id = new Guid(parts[1]);

            return
                (DomainEntity)
                _items.SingleOrDefault(
                    e =>
                    e.GetType().Name.ToLower() == typeName.ToLower() && e is DomainEntity && ((DomainEntity)e).Id == id);
        }

        public T Find<T>(Guid id) where T : Entity
        {
            return Query<T>(t => t.Id == id).FirstOrDefault();
        }

        public T Find<T>(string text) where T : Entity
        {
            return text.IsEmpty() ? null : _finder.Find<T>(this, text);
        }

        public void Delete(object target)
        {
            _items.Remove(target);
            _deletedItems.Add(target);
        }

        public void RejectChanges(object target)
        {
            _savedItems.Remove(target);
            _deletedItems.Remove(target);
            _revertedItems.Add(target);
        }

        public void FlushChanges()
        {
        }

        public IQueryable<T> Query<T>()
        {
            return findItems<T>();
        }

        public IQueryable<T> Query<T>(Expression<Func<T, bool>> where)
        {
            return findItems<T>().Where(where);
        }

        public IQueryable<T> Query<T>(IQueryExpression<T> queryExpression) where T : DomainEntity
        {
            return queryExpression.Apply(Query<T>());
        }

        public T FindBy<T, U>(Expression<Func<T, U>> expression, U search) where T : class
        {
            var accessor = ReflectionHelper.GetProperty(expression);

            return Query<T>(t => search.Equals((U)accessor.GetValue(t, null))).FirstOrDefault();
        }

        public T FindBy<T>(Expression<Func<T, bool>> where)
        {
            return Query(where).FirstOrDefault();
        }

        public void Save(object target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            _onSaveActions.Each(action => action(target));
            var entity = target as DomainEntity;
            if (entity != null && entity.IsNew())
            {
                lock (_lockObject)
                {
                    var nextId = Guid.NewGuid();
                    _nextId = nextId;
                    entity.Id = nextId;
                }
            }

            if (!_items.Contains(target))
            {
                _items.Add(target);
            }

            _savedItems.Add(target);
        }


        public void Insert(object target)
        {
            Save(target);
        }


        public T[] GetAll<T>()
        {
            return Query<T>(t => true).ToArray();
        }

        public IQueryable<T> RestrictedQuery<T>() where T : DomainEntity
        {
            throw new NotSupportedException();
        }

        private IQueryable<T> findItems<T>()
        {
            return _items.Where(e => e is T).Cast<T>().AsQueryable();
        }

        public T FindBy<T>(Expression<Func<T, bool>> where, bool forceRefresh)
        {
            return FindBy(where);
        }

        public void OnSave<T>(Action<T> action) where T : class
        {
            _onSaveActions.Add(o => { if (o is T) action((T)o); });
        }

        public void ClearHistory()
        {
            _savedItems.Clear();
            _deletedItems.Clear();
            _revertedItems.Clear();
        }
    }
}