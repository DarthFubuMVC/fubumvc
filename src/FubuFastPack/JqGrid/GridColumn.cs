using System;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Urls;

namespace FubuFastPack.JqGrid
{
    public class GridColumn : IGridColumn
    {
        private readonly Accessor _accessor;
        private readonly Expression _expression;

        public GridColumn(Accessor accessor, Expression expression)
        {
            _accessor = accessor;
            _expression = expression;
            FetchMode = ColumnFetching.FetchAndDisplay;
        }

        public Accessor Accessor
        {
            get { return _accessor; }
        }

        public Expression Expression
        {
            get { return _expression; }
        }

        public GridColumnDTO ToDto()
        {
            throw new NotImplementedException();
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
    }
}