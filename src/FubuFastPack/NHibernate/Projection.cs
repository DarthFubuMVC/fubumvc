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
        private readonly IList<ProjectionColumn<T>> _columns = new List<ProjectionColumn<T>>();
        private readonly ISession _session;
        private readonly List<ICriterion> _wheres = new List<ICriterion>();

        public Projection(ISession session)
        {
            MaxCount = INITIAL_MAX_PAGE_COUNT;
            SortBy = SortRule<T>.Ascending(x => x.Id);
            _session = session;
        }

        public int WhereCount
        {
            get { return _wheres.Count(); }
        }

        public SortRule<T> SortBy { get; set; }

        public int MaxCount { get; set; }

        void IDataSourceFilter<T>.WhereEqual(Expression<Func<T, object>> property, object value)
        {
            Where(property).IsEqualTo(value);
        }

        void IDataSourceFilter<T>.WhereNotEqual(Expression<Func<T, object>> property, object value)
        {
            Where(property).IsNot(value);
        }

        public int Count()
        {
            return Count(c => c);
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

        public void AddColumn(ProjectionColumn<T> column)
        {
            _columns.Add(column);
        }

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
            var criteria = _session.CreateCriteria(typeof (T));
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
            var criteria = _session.CreateCriteria(typeof (T));
            criteria = _columns.AddAliases(criteria);
            criteria = addWheres(criteria);
            return criteria;
        }

        public ICriteria AddSorting(ICriteria criteria, GridDataRequest page)
        {
            SortBy.ApplyDefaultSorting(page);

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
            var projections = Projections.ProjectionList();

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


        public void AddRestriction(ICriterion criterion)
        {
            _wheres.Add(criterion);
        }

        public WhereExpression Where(Expression<Func<T, object>> expression)
        {
            return new WhereExpression(expression, _wheres);
        }


        // TODO -- Reuse the CriterionBuilder here 

        public int Count(Func<ICriteria, ICriteria> chain)
        {
            var criteria = criteriaForCount(chain);
            var count = (int) criteria.UniqueResult();
            return count;
        }

        public void CountsOf<U>(Expression<Func<T, U>> property, Action<U, int> callback)
        {
            var criteria = _session.CreateCriteria(typeof (T));
            criteria = addWheres(criteria);

            var propertyName = property.ToPropertyName();
            var projections = Projections.ProjectionList();
            projections.Add(Projections.GroupProperty(propertyName));
            projections.Add(Projections.Count(propertyName));


            criteria = criteria.SetProjection(projections);


            criteria.List().Cast<object[]>().Each(o => { callback((U) o.GetValue(0), (int) o.GetValue(1)); });
        }

        protected ICriteria criteriaForCount(Func<ICriteria, ICriteria> chain)
        {
            var criteria = _session.CreateCriteria(typeof (T));
            criteria = _columns.AddAliases(criteria);
            criteria = addWheres(criteria);
            criteria = chain(criteria);

            var propertyName = _columns.Any() ? _columns[0].PropertyName : "Id";
            criteria = criteria.SetProjection(Projections.Count(propertyName));
            return criteria;
        }

        public void OuterJoin(Accessor accessor)
        {
            var column = _columns.FirstOrDefault(c => c.PropertyAccessor == accessor);
            if (column != null)
            {
                column.OuterJoin = true;
            }
        }

        public void AddWhere(ICriterion criterion)
        {
            _wheres.Add(criterion);
        }

        #region Nested type: AndExpression

        public interface AndExpression
        {
            WhereExpression And(Expression<Func<T, object>> expression);
        }

        #endregion

        #region Nested type: WhereExpression

        public class WhereExpression : AndExpression
        {
            private readonly List<ICriterion> _wheres;
            private Accessor _lastAccessor;

            public WhereExpression(Expression<Func<T, object>> expression, List<ICriterion> wheres)
            {
                _wheres = wheres;
                _lastAccessor = ReflectionHelper.GetAccessor(expression);
            }

            public WhereExpression And(Expression<Func<T, object>> expression)
            {
                _lastAccessor = ReflectionHelper.GetAccessor(expression);
                return this;
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

            public AndExpression IsNot(object value)
            {
                var criterion = Restrictions.Not(Restrictions.Eq(_lastAccessor.ToPropertyName(), value));
                _wheres.Add(criterion);
                return this;
            }

            public AndExpression IsNull()
            {
                var criterion = Restrictions.IsNull(_lastAccessor.ToPropertyName());
                _wheres.Add(criterion);
                return this;
            }

            public AndExpression IsOnOrAfter(DateTime date)
            {
                var criterion = Restrictions.Ge(_lastAccessor.ToPropertyName(), date);
                _wheres.Add(criterion);
                return this;
            }

            public AndExpression IsOnOrBefore(DateTime date)
            {
                var criterion = Restrictions.Le(_lastAccessor.ToPropertyName(), date);

                _wheres.Add(criterion);
                return this;
            }
        }

        #endregion
    }
}