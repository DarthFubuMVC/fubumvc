using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuFastPack.Domain;
using FubuFastPack.Querying;
using NHibernate;
using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.SqlCommand;

namespace FubuFastPack.NHibernate
{
    public class ProjectionColumn<T> where T : DomainEntity
    {
        protected Accessor _accessor;

        protected Accessor PropertyAccessor
        {
            get { return _accessor; }
        }

        public ProjectionColumn(Expression<Func<T, object>> expression)
            : this(expression.ToAccessor())
        {

        }

        public ProjectionColumn(Accessor accessor)
        {
            _accessor = accessor;
            if (_accessor != null) PropertyName = _accessor.Name;
        }

        public string PropertyName { get; set; }
        public bool OuterJoin { get; set; }

        public virtual void AddProjection(ProjectionList projections)
        {
            string propertyName = String.Join(".", _accessor.PropertyNames);
            var projection = Projections.Property(propertyName).As(_accessor.Name);
            projections.Add(projection);
        }

        public virtual void AddAlias(Cache<string, bool> aliasAndJoinTypeMap)
        {
            var key = _accessor.PropertyNames[0];

            if (_accessor.PropertyNames.Length <= 1 || aliasAndJoinTypeMap.Has(key)) return;

            aliasAndJoinTypeMap[key] = OuterJoin;
        }
    }

    public class DtoProjection<TEntity, TDto>
        where TEntity : DomainEntity
        where TDto : class, new()
    {
        private readonly Projection<TEntity> _projection;
        private readonly ProjectionRequestData _data = new ProjectionRequestData();

        public DtoProjection(ISession session)
        {
            _projection = new Projection<TEntity>(session);
        }

        public PropertyExpression From(Expression<Func<TEntity, object>> expression)
        {
            return new PropertyExpression(this, expression);
        }


        public class PropertyExpression
        {
            private readonly DtoProjection<TEntity, TDto> _parent;
            private ProjectionColumn<TEntity> _fromColumn;

            public PropertyExpression(DtoProjection<TEntity, TDto> parent, Expression<Func<TEntity, object>> from)
            {
                _parent = parent;
                _fromColumn = parent._projection.AddColumn(from);
            }

            public void To(Expression<Func<TDto, object>> expression)
            {
                _parent._data.AddPropertyName(expression.Name);
            }
        }
    }

    public class ProjectionRequestData : IRequestData
    {
        private readonly Cache<string, int> _positions = new Cache<string, int>();
        private object[] _objects;

        public void AddPropertyName(string propertyName)
        {
            _positions[propertyName] = _positions.Count;
        }

        public void SetObjects(object[] objects)
        {
            _objects = objects;
        }

        public object Value(string key)
        {
            return _objects[_positions[key]];
        }

        public bool Value(string key, Action<object> callback)
        {
            _positions.WithValue(key, i => callback(_objects[i]));
            return true;
        }

        public bool HasValueThatStartsWith(string key)
        {
            //TODO: provide implementation if wanted
            return false;
        }
    }


    public interface IProjection
    {
        int Count();
    }


    public interface IWhere
    {
        ICriterion Create();
    }


    public static class ProjectionExtensions
    {
        public static ICriteria AddAliases<T>(this IList<ProjectionColumn<T>> columns, ICriteria criteria) where T : DomainEntity
        {
            var aliasAndJoinTypeMap = new Cache<string, bool>();
            columns.Each(c => c.AddAlias(aliasAndJoinTypeMap));
            aliasAndJoinTypeMap.Each((alias, useOuterJoin) =>
            {
                JoinType joinType = useOuterJoin ? JoinType.LeftOuterJoin : JoinType.InnerJoin;
                criteria = criteria.CreateAlias(alias, alias, joinType);
            });

            return criteria;
        }
    }

    public class SortRule<T> where T : DomainEntity
    {
        private bool _ascending = true;
        private string _fieldName = "Id";

        private SortRule() { }

        public static SortRule<T> Ascending(Expression<Func<T, object>> property)
        {
            string fieldName = ReflectionHelper.GetProperty(property).Name;
            return new SortRule<T>() { _ascending = true, _fieldName = fieldName };
        }

        public static SortRule<T> Descending(Expression<Func<T, object>> property)
        {
            string fieldName = ReflectionHelper.GetProperty(property).Name;
            return new SortRule<T>() { _ascending = false, _fieldName = fieldName };
        }

        public void ModifyPaging(PagingOptions paging)
        {
            if (paging.SortColumn.IsNotEmpty()) return;

            paging.SortColumn = _fieldName;
            paging.SortAscending = _ascending;
        }

        public string FieldName { get { return _fieldName; } }
        public bool IsAscending { get { return _ascending; } }
    }

    public class Projection<T> : IProjection, IDataSourceFilter<T> where T : DomainEntity
    {
        private const int INITIAL_MAX_PAGE_COUNT = 1000;
        private readonly ISession _session;
        private readonly List<ICriterion> _wheres = new List<ICriterion>();
        private readonly IList<ProjectionColumn<T>> _columns = new List<ProjectionColumn<T>>();

        public Projection(ISession session)
        {
            MaxCount = INITIAL_MAX_PAGE_COUNT;
            SortBy = SortRule<T>.Ascending(x => x.Id);
            _session = session;
        }

        public ProjectionColumn<T> AddColumn(Accessor accessor)
        {
            var column = new ProjectionColumn<T>(accessor);
            _columns.Add(column);

            return column;
        }

        public int WhereCount { get { return _wheres.Count(); } }

        public void AddColumn(ProjectionColumn<T> column)
        {
            _columns.Add(column);
        }

        public SortRule<T> SortBy { get; set; }

        public int MaxCount { get; set; }

        public ProjectionColumn<T> AddColumn(Expression<Func<T, object>> expression)
        {
            return AddColumn(ReflectionHelper.GetAccessor(expression));
        }

        public IEnumerable<T> GetData(PagingOptions page)
        {
            return assembleCriteria(page, false).List<T>();
        }

        public IEnumerable GetAllData()
        {
            ICriteria criteria = _session.CreateCriteria(typeof(T));
            criteria = _columns.AddAliases(criteria);
            criteria = addWheres(criteria);
            criteria = AddTheProjections(criteria);


            return criteria.List();
        }

        private ICriteria assembleCriteria(PagingOptions page, bool withProjection)
        {
            var criteria = GetFilteredCriteria();
            if (withProjection) criteria = AddTheProjections(criteria);
            criteria = AddSorting(criteria, page);
            return AddPaging(criteria, page);
        }

        public ICriteria GetFilteredCriteria()
        {
            ICriteria criteria = _session.CreateCriteria(typeof(T));
            criteria = _columns.AddAliases(criteria);
            criteria = addWheres(criteria);
            return criteria;
        }

        public ICriteria AddSorting(ICriteria criteria, PagingOptions page)
        {
            SortBy.ModifyPaging(page);

            criteria = criteria.AddOrder(new Order(page.SortColumn, page.SortAscending));
            return criteria;
        }

        public ICriteria AddPaging(ICriteria criteria, PagingOptions page)
        {
            if (page.ResultsPerPage > MaxCount)
            {
                page.ResultsPerPage = MaxCount;
            }

            criteria = criteria.SetFirstResult(page.ResultsToSkip()).SetMaxResults(page.ResultsPerPage);
            return criteria;
        }

        public IList ExecuteCriteriaWithProjection(PagingOptions page)
        {
            return assembleCriteria(page, true).List();
        }

        public ICriteria AddTheProjections(ICriteria criteria)
        {
            ProjectionList projections = Projections.ProjectionList();

            _columns.Each(column => column.AddProjection(projections));

            return criteria.SetProjection(projections);
        }

        protected virtual ICriteria addWheres(ICriteria criteria)
        {
            foreach (ICriterion criterion in _wheres)
            {
                criteria = criteria.Add(criterion);
            }

            return criteria;
        }

        public WhereExpression Where(Expression<Func<T, object>> expression)
        {
            return new WhereExpression(expression, _wheres);
        }

        public class WhereExpression : AndExpression
        {
            private readonly List<ICriterion> _wheres;
            private Accessor _lastAccessor;

            public WhereExpression(Expression<Func<T, object>> expression, List<ICriterion> wheres)
            {
                _wheres = wheres;
                _lastAccessor = ReflectionHelper.GetAccessor(expression);
            }

            public AndExpression IsEqualTo(object value)
            {
                var criterion = Restrictions.Eq(getPropertyName(), value);
                _wheres.Add(criterion);

                return this;
            }

            public AndExpression StartsWith(string beginning)
            {
                var criterion = Restrictions.InsensitiveLike(getPropertyName(), beginning + "%");
                _wheres.Add(criterion);
                return this;
            }

            private string getPropertyName()
            {
                return _lastAccessor.PropertyNames.Join(".");
            }

            public AndExpression IsNotIs(object value)
            {
                var criterion = Restrictions.Not(Restrictions.Eq(getPropertyName(), value));
                _wheres.Add(criterion);

                return this;
            }

            public AndExpression IsIn(object[] collection)
            {
                var criterion = Restrictions.In(getPropertyName(), collection);
                _wheres.Add(criterion);

                return this;
            }

            public AndExpression ContainsCaseInsensitive(object value)
            {
                var criterion = Restrictions.InsensitiveLike(getPropertyName(), "%" + value + "%");
                _wheres.Add(criterion);
                return this;
            }

            public WhereExpression And(Expression<Func<T, object>> expression)
            {
                _lastAccessor = ReflectionHelper.GetAccessor(expression);
                return this;
            }

            public AndExpression IsNot(object value)
            {
                var criterion = Restrictions.Not(Restrictions.Eq(_lastAccessor.Name, value));
                _wheres.Add(criterion);
                return this;
            }

            public AndExpression IsNull()
            {
                var criterion = Restrictions.IsNull(_lastAccessor.Name);
                _wheres.Add(criterion);
                return this;
            }

            public AndExpression IsOnOrAfter(DateTime date)
            {
                var criterion = Restrictions.Ge(_lastAccessor.Name, date);
                _wheres.Add(criterion);
                return this;
            }

            public AndExpression IsOnOrBefore(DateTime date)
            {
                var criterion = Restrictions.Le(_lastAccessor.Name, date);

                _wheres.Add(criterion);
                return this;
            }
        }

        public interface AndExpression
        {
            WhereExpression And(Expression<Func<T, object>> expression);
        }

        public int Count()
        {
            return Count(c => c);
        }

        public int Count(Func<ICriteria, ICriteria> chain)
        {
            ICriteria criteria = criteriaForCount(chain);
            var count = (int)criteria.UniqueResult();
            return count;
        }

        public void CountsOf<U>(Expression<Func<T, U>> property, Action<U, int> callback)
        {
            ICriteria criteria = _session.CreateCriteria(typeof(T));
            criteria = addWheres(criteria);

            var propertyName = property.ToPropertyName();
            ProjectionList projections = Projections.ProjectionList();
            projections.Add(Projections.GroupProperty(propertyName));
            projections.Add(Projections.Count(propertyName));


            criteria = criteria.SetProjection(projections);


            criteria.List().Cast<object[]>().Each(o =>
            {
                callback((U)o.GetValue(0), (int)o.GetValue(1));
            });
        }

        protected ICriteria criteriaForCount(Func<ICriteria, ICriteria> chain)
        {
            ICriteria criteria = _session.CreateCriteria(typeof(T));
            //criteria = addAliases(criteria);
            criteria = addWheres(criteria);
            criteria = chain(criteria);

            criteria = criteria.SetProjection(Projections.Count(_columns[0].PropertyName));
            return criteria;
        }

        void IDataSourceFilter<T>.WhereEqual(Expression<Func<T, object>> property, object value)
        {
            Where(property).IsEqualTo(value);
        }

        void IDataSourceFilter<T>.WhereNotEqual(Expression<Func<T, object>> property, object value)
        {
            Where(property).IsNot(value);
        }
    }
}