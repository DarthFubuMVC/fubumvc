using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuFastPack.Domain;
using FubuFastPack.Querying;
using FubuLocalization;
using Microsoft.Practices.ServiceLocation;

namespace FubuFastPack.JqGrid
{
    public class SortOrder
    {
        public string sortname { get; set; }
        public string sortorder { get; set; }
    }

    public abstract class Grid<TEntity, TService> : ISmartGrid<TEntity> where TEntity : DomainEntity
    {
        private readonly GridDefinition<TEntity> _definition = new GridDefinition<TEntity>();
        private readonly IList<Action<IDictionary<string, object>>> _colModelModifications 
            = new List<Action<IDictionary<string, object>>>();


        protected Action<IDictionary<string, object>> modifyColumnModel
        {
            set
            {
                _colModelModifications.Add(value);
            }
        }

        protected StringToken Header { get; set; }

        public virtual string GetHeader()
        {
            if (Header == null)
            {
                return LocalizationManager.GetPluralTextForType(typeof (TEntity));
            }

            return Header.ToString();
        }

        public T AddColumn<T>(T column) where T : IGridColumn
        {
            return _definition.AddColumn(column);
        }

        public void DisableLinks()
        {
            _definition.Columns.OfType<LinkColumn<TEntity>>().Each(x => x.DisableLink());
        }

        private readonly IList<Criteria> _initialCriteria = new List<Criteria>();
        public void AddCriteria(Expression<Func<TEntity, object>> property, StringToken op, object value)
        {
            _initialCriteria.Add(Criteria.For(property, op.Key, value.ToString()));
        }
        public IEnumerable<Criteria> InitialCriteria()
        {
            return _initialCriteria;
        }

        public GridResults Invoke(IServiceLocator services, GridDataRequest request)
        {
            var runner = services.GetInstance<IGridRunner<TEntity, TService>>();
            var source = BuildSource(runner.Service);

            return runner.RunGrid(_definition, source, request);
        }

        public int Count(IServiceLocator services, Action<IGridDataSource<TEntity>> configure)
        {
            var runner = services.GetInstance<IGridRunner<TEntity, TService>>();
            var source = BuildSource(runner.Service);
            configure(source);

            return runner.GetCount(source);
        }

        public int Count(IServiceLocator services)
        {
            return Count(services, x => { });
        }

        public int Count(IServiceLocator services, IDataRestriction<TEntity> restriction)
        {
            return Count(services, x => x.ApplyRestrictions(restriction.Apply));
        }

        public IEnumerable<FilteredProperty> AllFilteredProperties(IQueryService queryService)
        {
            // Force the enumerable to execute so we don't keep building new FilteredProperty objects
            // *has* to be distinct
            var properties = _definition.Columns.SelectMany(x => x.FilteredProperties()).Distinct().ToList();
            properties.Each(x => x.Operators = queryService.FilterOptionsFor<TEntity>(x.Accessor));
            return properties;
        }

        public IEnumerable<IDictionary<string, object>> ToColumnModel()
        {
            var columns = _definition.Columns.SelectMany(x => x.ToDictionary()).ToList();
            columns.Each(c => _colModelModifications.Each(m => m(c)));

            return columns;
        }

        public IGridDefinition Definition
        {
            get { return _definition; }
        }

        public void SortAscending(Expression<Func<TEntity, object>> property)
        {
            _definition.SortBy = SortRule<TEntity>.Ascending(property);
        }

        public void SortDescending(Expression<Func<TEntity, object>> property)
        {
            _definition.SortBy = SortRule<TEntity>.Descending(property);
        }

        public CommandColumn<TEntity, TInputModel> ShowItemActionLink<TInputModel>(StringToken key)
        {
            var column = new CommandColumn<TEntity, TInputModel>(key);
            _definition.AddColumn(column);

            return column;
        }

        public void LimitRowsTo(int count)
        {
            _definition.MaxCount = count;
        }

        public FilterColumn<TEntity> FilterOn(Expression<Func<TEntity, object>> expression)
        {
            return _definition.AddColumn(new FilterColumn<TEntity>(expression));
        }

        public GridColumn<TEntity> Show(Expression<Func<TEntity, object>> expression)
        {
            return _definition.Show(expression);
        }

        public LinkColumn<TEntity> ShowViewLink(Expression<Func<TEntity, object>> expression)
        {
            return _definition.ShowViewLink(expression);
        }

        public GridDefinition<TEntity>.OtherEntityLinkExpression<TOther> ShowViewLinkForOther<TOther>(
            Expression<Func<TEntity, TOther>> entityProperty) where TOther : DomainEntity
        {
            return _definition.ShowViewLinkForOther(entityProperty);
        }

        public abstract IGridDataSource<TEntity> BuildSource(TService service);

        public void DoNotAllowUserSorting()
        {
            modifyColumnModel = dict =>
            {
                if (dict.ContainsKey("sortable"))
                {
                    dict["sortable"] = false;
                }
                else
                {
                    dict.Add("sortable", false);
                }
            };
        }

        public void AllowCreateNew()
        {
            _definition.AllowCreationOfNew = true;
        }

        public void CanSaveQuery(bool canSave)
        {
            _definition.CanSaveQuery = canSave;
        }

        public Type EntityType
        {
            get { return typeof (TEntity); }
        }

        public void ApplyPolicies(IEnumerable<IGridPolicy> policies)
        {
            policies.Each(p =>
            {
                p.AlterGrid(this);
                p.AlterDefinition(_definition);
            });
            
        }

        public SortOrder SortOrder()
        {
            return _definition.SortOrder();
        }
    }
}