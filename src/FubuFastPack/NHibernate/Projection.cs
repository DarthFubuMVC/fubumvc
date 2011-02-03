using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuFastPack.Domain;
using FubuFastPack.Querying;
using NHibernate;
using NHibernate.Criterion;

namespace FubuFastPack.NHibernate
{
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

        public IEnumerable<Accessor> SelectAccessors()
        {
            return _columns.Select(x => x.PropertyAccessor);
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
            Debug.WriteLine("Adding column " + expression.ToAccessor().Name);
            return AddColumn(ReflectionHelper.GetAccessor(expression));
        }

        public IEnumerable<T> GetData(GridDataRequest page)
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

        private ICriteria assembleCriteria(GridDataRequest page, bool withProjection)
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

        public ICriteria AddSorting(ICriteria criteria, GridDataRequest page)
        {
            SortBy.ModifyPaging(page);

            criteria = criteria.AddOrder(new Order(page.SortColumn, page.SortAscending));
            return criteria;
        }

        public ICriteria AddPaging(ICriteria criteria, GridDataRequest page)
        {
            if (page.ResultsPerPage > MaxCount)
            {
                page.ResultsPerPage = MaxCount;
            }

            criteria = criteria.SetFirstResult(page.ResultsToSkip()).SetMaxResults(page.ResultsPerPage);
            return criteria;
        }

        public IList ExecuteCriteriaWithProjection(GridDataRequest page)
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