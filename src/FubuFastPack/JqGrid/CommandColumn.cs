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
        private readonly StringToken _header;
        private StringToken _body;
        private Accessor _accessor;
        private string _linkName;
        private string _formatter = "command";

        public CommandColumn(StringToken header)
        {
            _header = header;
            _body = header;
            _accessor = ReflectionHelper.GetAccessor<TEntity>(x => x.Id);
            _linkName = "linkFor" + _header.Key;
        }

        public CommandColumn<TEntity, TInputType> Body(StringToken body)
        {
            _body = body;
            return this;
        }

        public CommandColumn<TEntity, TInputType> Formatter(string formatterName)
        {
            _formatter = formatterName;
            return this;
        }

        public IEnumerable<IDictionary<string, object>> ToDictionary()
        {
            yield return new Dictionary<string, object>{
                {"formatter", _formatter},
                {"name", _header.Key},
                {"index", _header.Key},
                {"sortable", false},
                {"linkName", _linkName},

            };
        }

        public Action<EntityDTO> CreateDtoFiller(IGridData data, IDisplayFormatter formatter, IUrlRegistry urls)
        {
            var idSource = data.GetterFor(_accessor);

            return dto =>
            {
                dto.AddCellDisplay(_body.ToString());

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

        public IEnumerable<string> Headers()
        {
            yield return GetHeader();
        }

        public string GetHeader()
        {
            return _header.ToString();
        }

        public bool IsOuterJoin
        {
            get; set;
        }
    }
}