using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Urls;

namespace FubuFastPack.JqGrid
{
    public class GridColumn<T> : GridColumnBase<T>, IGridColumn
    {
        public static GridColumn<T> ColumnFor(Expression<Func<T, object>> property)
        {
            return new GridColumn<T>(property);
        }

        public GridColumn(Expression<Func<T, object>> expression) : base(expression)
        {
            IsSortable = true;
        }

        public IEnumerable<Accessor> SelectAccessors()
        {
            yield return Accessor;
        }

        public IEnumerable<Accessor> AllAccessors()
        {
            yield return Accessor;
        }

        // TODO -- UT this.  Duh.
        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>{
                {"name", Accessor.Name},
                {"index", Accessor.Name},
                {"sortable", IsSortable}
            };
        }

        public Action<EntityDTO> CreateFiller(IGridData data, IDisplayFormatter formatter, IUrlRegistry urls)
        {
            var source = data.GetterFor(Accessor);

            //if (FetchMode == ColumnFetching.FetchOnly)
            //{
            //    return dto =>
            //    {
            //        var rawValue = source();
            //        dto[Accessor.Name] = rawValue == null ? string.Empty : rawValue.ToString();
            //    };
            //}

            // TODO -- later, this will do formatting stuff too
            return dto =>
            {
                var rawValue = source();

                dto.AddCellDisplay(formatter.GetDisplayForValue(Accessor, rawValue));
            };
        }
    }
}