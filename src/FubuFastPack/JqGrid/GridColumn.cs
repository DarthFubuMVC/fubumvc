using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuFastPack.Domain;
using FubuFastPack.Querying;
using FubuMVC.Core.Urls;

namespace FubuFastPack.JqGrid
{

    public class DataColumn<T> : IGridColumn where T : DomainEntity
    {
        public IDictionary<string, object> ToDictionary()
        {
            var dictionary = new Dictionary<string, object>();
            dictionary.Add("name", "data");
            dictionary.Add("index", "data");
            dictionary.Add("sortable", false);
            dictionary.Add("hidden", true);

            return dictionary;
        }

        

        public Action<EntityDTO> CreateFiller(IGridData data, IDisplayFormatter formatter, IUrlRegistry urls)
        {
            var accessor = ReflectionHelper.GetAccessor<T>(x => x.Id);
            var getter = data.GetterFor(accessor);
            return dto => dto["Id"] = getter().ToString();
        }

        public IEnumerable<FilterDTO> PossibleFilters(IQueryService queryService)
        {
            yield break;
        }

        public IEnumerable<Accessor> SelectAccessors()
        {
            yield break;
        }

        public string GetHeader()
        {
            return "Data";
        }
    }


    public class GridColumn<T> : GridColumnBase<T>, IGridColumn
    {
        public static GridColumn<T> ColumnFor(Expression<Func<T, object>> property)
        {
            return new GridColumn<T>(property.ToAccessor(), property);
        }

        public GridColumn(Accessor accessor, Expression<Func<T, object>> expression) : base(accessor, expression)
        {
            FetchMode = ColumnFetching.FetchAndDisplay;
            IsSortable = true;
        }

        public IEnumerable<Accessor> SelectAccessors()
        {
            if (FetchMode == ColumnFetching.NoFetch) yield break;

            yield return Accessor;
        }

        public ColumnFetching FetchMode { get; set; }
        public bool IsSortable { get; set; }

        // TODO -- UT this.  Duh.
        public IDictionary<string, object> ToDictionary()
        {
            var dictionary = new Dictionary<string, object>();

            dictionary.Add("name", Accessor.Name);
            dictionary.Add("index", Accessor.Name);
            dictionary.Add("sortable", IsSortable);

            return dictionary;
        }

        public Action<EntityDTO> CreateFiller(IGridData data, IDisplayFormatter formatter, IUrlRegistry urls)
        {
            if (FetchMode == ColumnFetching.NoFetch) return dto => { };

            var source = data.GetterFor(Accessor);

            if (FetchMode == ColumnFetching.FetchOnly)
            {
                return dto =>
                {
                    var rawValue = source();
                    dto[Accessor.Name] = rawValue == null ? string.Empty : rawValue.ToString();
                };
            }

            // TODO -- later, this will do formatting stuff too
            return dto =>
            {
                var rawValue = source();

                dto.AddCellDisplay(formatter.GetDisplayForValue(Accessor, rawValue));
            };
        }
    }
}