using System;
using System.Linq;
using FubuCore.Util;
using FubuFastPack.Domain;
using FubuFastPack.NHibernate;
using FubuFastPack.Querying;
using FubuLocalization;
using NHibernate.Criterion;
using FubuCore;

namespace FubuFastPack.JqGrid
{
    public class ProjectionDataSource<T> : IGridDataSource<T> 
        where T : DomainEntity
    {
        private readonly Projection<T> _projection;
        private readonly ProjectionSelectMode _selectMode;

        public ProjectionDataSource(Projection<T> projection, ProjectionSelectMode selectMode)
        {
            _projection = projection;
            _selectMode = selectMode;
        }

        public int TotalCount()
        {
            return _projection.Count();
        }

        public IGridData Fetch(GridDataRequest options)
        {
            var records = _projection.ExecuteCriteriaWithProjection(options).Cast<object>().ToList();

            if (_selectMode == ProjectionSelectMode.OnlySpecifiedColumns)
            {
                var accessors = _projection.SelectAccessors().ToList();
                return new ProjectionGridData<T>(records, accessors);
            }

            return new EntityGridData(records);
        }

        public void ApplyRestrictions(Action<IDataSourceFilter<T>> configure)
        {
            configure(_projection);
        }

        public void ApplyCriteria(FilterRequest<T> request, IQueryService queryService)
        {
            var rule = queryService.FilterRuleFor(request);
            var criterion = CriterionBuilder.CriterionForRule(rule);
            _projection.AddWhere(criterion);    
        }
    }



    // TODO -- Tested through StoryTeller
    public static class CriterionBuilder
    {
        private static readonly Cache<StringToken, Func<FilterRule, ICriterion>> _builders
            = new Cache<StringToken, Func<FilterRule, ICriterion>>();

        static CriterionBuilder()
        {
            _builders[OperatorKeys.EQUAL] = rule => Restrictions.Eq(rule.Accessor.ToPropertyName(), rule.Value);
            _builders[OperatorKeys.NOTEQUAL] = rule => Restrictions.Not(Restrictions.Eq(rule.Accessor.ToPropertyName(), rule.Value));
            _builders[OperatorKeys.CONTAINS] = rule => Restrictions.InsensitiveLike(rule.Accessor.ToPropertyName(), (string)rule.Value, MatchMode.Anywhere);
            _builders[OperatorKeys.STARTSWITH] = rule => Restrictions.InsensitiveLike(rule.Accessor.ToPropertyName(), (string)rule.Value, MatchMode.Start);
            _builders[OperatorKeys.ENDSWITH] = rule => Restrictions.InsensitiveLike(rule.Accessor.ToPropertyName(), (string)rule.Value, MatchMode.End);
            _builders[OperatorKeys.LESSTHAN] = rule => Restrictions.Lt(rule.Accessor.ToPropertyName(), rule.Value);
            _builders[OperatorKeys.LESSTHANOREQUAL] = rule => Restrictions.Le(rule.Accessor.ToPropertyName(), rule.Value);
            _builders[OperatorKeys.GREATERTHAN] = rule => Restrictions.Gt(rule.Accessor.ToPropertyName(), rule.Value);
            _builders[OperatorKeys.GREATERTHANOREQUAL] = rule => Restrictions.Ge(rule.Accessor.ToPropertyName(), rule.Value);
        }

        public static ICriterion CriterionForRule(FilterRule rule)
        {
            if (_builders.Has(rule.Operator))
            {
                return _builders[rule.Operator](rule);
            }
            else
            {
                throw new ArgumentOutOfRangeException("The operator {0} is not recognized".ToFormat(rule.Operator.Key));
            }

        }
    }
}