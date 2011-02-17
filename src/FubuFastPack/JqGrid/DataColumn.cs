using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;
using FubuFastPack.Domain;
using FubuFastPack.Querying;
using FubuMVC.Core.Urls;

namespace FubuFastPack.JqGrid
{
    public class DataColumn<T> : IGridColumn where T : DomainEntity
    {
        private readonly Accessor _accessor;

        public DataColumn()
        {
            _accessor = ReflectionHelper.GetAccessor<T>(x => x.Id);
        }

        // TODO -- UT this
        public IEnumerable<IDictionary<string, object>> ToDictionary()
        {
            yield return new Dictionary<string, object>{
                {"name", "data"},
                {"index", "data"},
                {"sortable", false},
                {"hidden", true}
            };
        }

        public Action<EntityDTO> CreateDtoFiller(IGridData data, IDisplayFormatter formatter, IUrlRegistry urls)
        {
            var getter = data.GetterFor(_accessor);
            return dto => dto["Id"] = getter().ToString();
        }

        public IEnumerable<Accessor> SelectAccessors()
        {
            yield return _accessor;
        }

        public IEnumerable<Accessor> AllAccessors()
        {
            return SelectAccessors();
        }

        public IEnumerable<FilteredProperty> FilteredProperties()
        {
            yield break;
        }

        public IEnumerable<string> Headers()
        {
            yield return GetHeader();
        }

        public string GetHeader()
        {
            return "Data";
        }

        public bool IsOuterJoin
        {
            get; set;
        }
    }
}