using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuFastPack.Querying;
using FubuMVC.Core.Urls;

namespace FubuFastPack.JqGrid
{

    public interface IColumnFormatterStrategy
    {
        Func<object, string> ToFormatter(IDisplayFormatter formatter);
    }

    public class GridColumn<T> : GridColumnBase<T, GridColumn<T>>, IGridColumn, IColumnFormatterStrategy
    {
        private readonly IList<Action<IDictionary<string, object>>> _modifications = new List<Action<IDictionary<string, object>>>();
        private IColumnFormatterStrategy _formatterStrategy;

        public static GridColumn<T> ColumnFor(Expression<Func<T, object>> property)
        {
            return new GridColumn<T>(property);
        }

        public GridColumn(Expression<Func<T, object>> expression) : base(expression)
        {
            IsSortable = true;
            IsFilterable = true;

            _formatterStrategy = this;
        }

        private Action<IDictionary<string, object>> modifyColumnModel
        {
            set
            {
                _modifications.Add(value);
            }
        }

        public IEnumerable<Accessor> SelectAccessors()
        {
            yield return Accessor;
        }

        public IEnumerable<Accessor> AllAccessors()
        {
            yield return Accessor;
        }

        public IEnumerable<string> Headers()
        {
            yield return GetHeader();
        }

        public IEnumerable<IDictionary<string, object>> ToDictionary()
        {
            var dictionary = new Dictionary<string, object>{
                {"name", Accessor.Name},
                {"index", Accessor.Name},
                {"sortable", IsSortable}
            };

            _modifications.Each(m => m(dictionary));

            yield return dictionary;
        }

        public virtual Action<EntityDTO> CreateDtoFiller(IGridData data, IDisplayFormatter formatter, IUrlRegistry urls)
        {
            var source = data.GetterFor(Accessor);
            var toString = _formatterStrategy.ToFormatter(formatter);

            return dto =>
            {
                var rawValue = source();

                var displayForValue = toString(rawValue);
                dto.AddCellDisplay(displayForValue);
            };
        }

        Func<object, string> IColumnFormatterStrategy.ToFormatter(IDisplayFormatter formatter)
        {
            return o => formatter.GetDisplayForValue(Accessor, o);
        }

        public GridColumn<T> TimeAgo()
        {
            _formatterStrategy = new TimeAgoStrategy(Accessor);
            modifyColumnModel = dict => dict.Add("formatter", "timeAgo");
            return this;
        }
    }

    public class TimeAgoStrategy : IColumnFormatterStrategy
    {
        private readonly Accessor _accessor;

        public TimeAgoStrategy(Accessor accessor)
        {
            _accessor = accessor;
        }

        public Func<object, string> ToFormatter(IDisplayFormatter formatter)
        {
            return o =>
            {
                var request = new GetStringRequest(_accessor, o, null){
                    Format = "{0:s}"
                };
                return formatter.GetDisplay(request);
            };
        }
    }
}