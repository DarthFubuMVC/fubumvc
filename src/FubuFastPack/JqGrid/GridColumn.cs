using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuFastPack.Querying;
using FubuLocalization;
using FubuMVC.Core.Urls;
using System.Linq;

namespace FubuFastPack.JqGrid
{
    public class GridColumn<T> : IGridColumn
    {
        public static GridColumn<T> ColumnFor<T>(Expression<Func<T, object>> property)
        {
            return new GridColumn<T>(property.ToAccessor(), property);
        }

        private readonly Accessor _accessor;
        private readonly Expression<Func<T, object>> _expression;
        private StringToken _header;

        public GridColumn(Accessor accessor, Expression<Func<T, object>> expression)
        {
            _accessor = accessor;
            _expression = expression;
            FetchMode = ColumnFetching.FetchAndDisplay;
        }

        public Accessor Accessor
        {
            get { return _accessor; }
        }

        public GridColumnDTO ToDto()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FilterDTO> PossibleFilters(IQueryService queryService)
        {
            if (!IsFilterable) yield break;

            yield return new FilterDTO(){
                display = GetHeader(),
                value = _accessor.Name,
                operators = queryService.FilterOptionsFor(_expression).Select(x => x.ToOperator()).ToArray()
            };
        }

        public IEnumerable<Accessor> SelectAccessors()
        {
            if (FetchMode == ColumnFetching.NoFetch) yield break;

            yield return _accessor;
        }

        public ColumnFetching FetchMode { get; set; }
        public bool IsFilterable { get; set; }
        public bool IsSortable { get; set; }

        // TODO -- UT this
        public Action<EntityDTO> CreateFiller(IGridData data, IDisplayFormatter formatter, IUrlRegistry urls)
        {
            if (FetchMode == ColumnFetching.NoFetch) return dto => { };

            var source = data.GetterFor(_accessor);

            if (FetchMode == ColumnFetching.FetchOnly)
            {
                return dto =>
                {
                    var rawValue = source();
                    dto[_accessor.Name] = rawValue == null ? string.Empty : rawValue.ToString();
                };
            }

            // TODO -- later, this will do formatting stuff too
            return dto =>
            {
                var rawValue = source();

                dto.AddCellDisplay(formatter.GetDisplayForValue(_accessor, rawValue));
            };
        }

        public void OverrideHeader(StringToken token)
        {
            _header = token;
        }

        public string GetHeader()
        {
            if (_header != null) return _header.ToString();

            return LocalizationManager.GetHeader(_expression);
        }
    }
}