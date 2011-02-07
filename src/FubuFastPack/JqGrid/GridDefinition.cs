using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuFastPack.Domain;
using FubuFastPack.Querying;

namespace FubuFastPack.JqGrid
{
    public class GridDefinition<T> : IGridDefinition where T : DomainEntity
    {
        private readonly List<IGridColumn> _columns = new List<IGridColumn>();

        public GridDefinition()
        {
            _columns.Add(new DataColumn<T>());
        }

        public IEnumerable<IGridColumn> Columns
        {
            get { return _columns; }
        }

        public IEnumerable<Accessor> SelectedAccessors
        {
            get { return _columns.SelectMany(x => x.SelectAccessors()).Distinct(); }
        }

        public IEnumerable<FilterDTO> AllPossibleFilters(IQueryService queryService)
        {
            return _columns.SelectMany(x => x.PossibleFilters(queryService));
        }

        protected TColumn addColumn<TColumn>(TColumn column) where TColumn : IGridColumn
        {
            _columns.Add(column);
            return column;
        }

        public GridColumn<T> Show(Expression<Func<T, object>> expression)
        {
            return addColumn(new GridColumn<T>(expression));
        }

        public LinkColumn<T> ShowViewLink(Expression<Func<T, object>> expression)
        {
            return addColumn(new LinkColumn<T>(expression));
        }

        public OtherEntityLinkExpression<TOther> ShowViewLinkForOther<TOther>(Expression<Func<T, TOther>> entityProperty) where TOther : DomainEntity
        {
            return new OtherEntityLinkExpression<TOther>(this, entityProperty);
        }


        public void FilterOn(Expression<Func<T, object>> expression)
        {
            throw new NotImplementedException();
            //var column = new GridColumn<T>(expression)
            //{
            //    IsFilterable = true
            //};

            //_columns.Add(column);
        }

        public class OtherEntityLinkExpression<TOther> where TOther : DomainEntity
        {
            private readonly GridDefinition<T> _grid;
            private readonly Expression<Func<T, TOther>> _entityProperty;

            public OtherEntityLinkExpression(GridDefinition<T> grid, Expression<Func<T, TOther>> entityProperty)
            {
                _grid = grid;
                _entityProperty = entityProperty;
            }

            public LinkColumn<T> DisplayTextFrom(Expression<Func<TOther, object>> property)
            {
                var topAccessor = ReflectionHelper.GetAccessor(_entityProperty);
                var displayAccessor = topAccessor.GetChildAccessor(property);
                var idAccessor = topAccessor.GetChildAccessor<TOther>(x => x.Id);

                return _grid.addColumn(new LinkColumn<T>(displayAccessor, idAccessor, typeof(TOther)));
            } 
        }

        public Expression<Func<T, object>> PropertyExpressionFor(string propertyName)
        {
            return _columns.SelectMany(x => x.AllAccessors()).First(x => x.Name == propertyName).ToExpression<T>();
        }
    }
}