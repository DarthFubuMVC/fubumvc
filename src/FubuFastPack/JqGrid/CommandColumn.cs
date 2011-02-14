using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;
using FubuFastPack.Domain;
using FubuFastPack.Querying;
using FubuLocalization;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Urls;

namespace FubuFastPack.JqGrid
{
    public class CommandColumn<TEntity, TInputType> : IGridColumn where TEntity : DomainEntity
    {
        private readonly StringToken _key;
        private Accessor _accessor;
        private string _linkName;

        public CommandColumn(StringToken key)
        {
            _key = key;
            _accessor = ReflectionHelper.GetAccessor<TEntity>(x => x.Id);
            _linkName = "linkFor" + _key.Key;
        }

        public IEnumerable<IDictionary<string, object>> ToDictionary()
        {
            yield return new Dictionary<string, object>{
                {"formatter", "command"},
                {"name", _key.Key},
                {"index", _key.Key},
                {"sortable", false},
                {"linkName", _linkName},

            };
        }

        public Action<EntityDTO> CreateDtoFiller(IGridData data, IDisplayFormatter formatter, IUrlRegistry urls)
        {
            var idSource = data.GetterFor(_accessor);

            return dto =>
            {
                dto.AddCellDisplay(_key.ToString());

                var parameters = new RouteParameters();
                parameters[_accessor.Name] = idSource().ToString();

                var url = urls.UrlFor(typeof(TInputType), parameters);
                dto[_linkName] = url;
            };
        }

        public IEnumerable<Accessor> SelectAccessors()
        {
            yield return _accessor;
        }

        public IEnumerable<Accessor> AllAccessors()
        {
            yield return _accessor;
        }

        public IEnumerable<FilteredProperty> FilteredProperties()
        {
            yield break;
        }

        public string GetHeader()
        {
            return _key.ToString();
        }

        public bool IsOuterJoin
        {
            get; set;
        }
    }
}