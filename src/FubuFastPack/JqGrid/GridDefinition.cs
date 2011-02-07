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
            get { return _columns.SelectMany(x => x.SelectAccessors()); }
        }

        public IEnumerable<FilterDTO> AllPossibleFilters(IQueryService queryService)
        {
            return _columns.SelectMany(x => x.PossibleFilters(queryService));
        }

        protected void addColumn(IGridColumn column)
        {
            _columns.Add(column);
        }

        public GridColumn<T> Show(Expression<Func<T, object>> expression)
        {
            var column = new GridColumn<T>(expression);

            _columns.Add(column);

            return column;
        }

        public LinkColumn<T> ShowViewLink(Expression<Func<T, object>> expression)
        {
            var column = new LinkColumn<T>(expression);

            _columns.Add(column);

            return column;
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

        }

        public Expression<Func<T, object>> PropertyExpressionFor(string propertyName)
        {
            return _columns.SelectMany(x => x.AllAccessors()).First(x => x.Name == propertyName).ToExpression<T>();
        }
    }
}