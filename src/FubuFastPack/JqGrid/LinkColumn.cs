using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuFastPack.Domain;
using FubuLocalization;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Urls;

namespace FubuFastPack.JqGrid
{
    /*
     *  LATER================>
     *  1.) need to track other parameters for the Url
     *  2.) Link does filter on the display property
     *  3.) needs to chain a header
     * 
     *  Maybe pull out a base class to share with GridColumn<T> for the 
     *  ToDTO bit
     * 
     * 
     */

    // TODO -- need to add other accessors for getting the Url?
    // TODO -- way to override the link name?
    // TODO -- move the ctor's to static factory methods
    public class LinkColumn<T> : GridColumnBase<T, LinkColumn<T>>, IGridColumn where T : DomainEntity
    {
        private readonly Accessor _idAccessor;
        private string _linkName;
        private readonly Type _inputModelType;
        private StringToken _literalText;
        private bool _disabled;

        public LinkColumn(Accessor accessor, Type inputModelType, StringToken literalText) : base(accessor)
        {
            _inputModelType = inputModelType;
            _literalText = literalText;
        }

        public LinkColumn(Expression<Func<T, object>> expression) : base(expression)
        {
            _idAccessor = ReflectionHelper.GetAccessor<T>(x => x.Id);
            _inputModelType = typeof (T);
            initialize();
        }

        public LinkColumn(Accessor displayAccessor, Accessor idAccessor, Type inputModelType) : base(displayAccessor)
        {
            _idAccessor = idAccessor;
            _inputModelType = inputModelType;
            initialize();
        }
        
        // TODO -- UT this little monster
        public IEnumerable<IDictionary<string, object>> ToDictionary()
        {
            var dictionary = new Dictionary<string, object>{
                {"name", Accessor.Name},
                {"index", Accessor.Name},
                {"sortable", IsSortable},
                {"linkName", _linkName},
                {"formatter", "link"}
            };

            if (_disabled)
            {
                dictionary.Remove("formatter");
            }

            yield return dictionary;
        }

        public void DisableLink()
        {
            _disabled = true;
        }

        public LinkColumn<T> LiteralText(StringToken literal)
        {
            _literalText = literal;
            return this;
        }

        public Action<EntityDTO> CreateDtoFiller(IGridData data, IDisplayFormatter formatter, IUrlRegistry urls)
        {
            var displaySource = data.GetterFor(Accessor);
            var idSource = data.GetterFor(_idAccessor);

            if (_disabled)
            {
                return dto =>
                {
                    var rawValue = displaySource();
                    var display = _literalText == null ? formatter.GetDisplayForValue(Accessor, rawValue) : _literalText.ToString();
                    dto.AddCellDisplay(display);
                };
            }

            return dto =>
            {
                var rawValue = displaySource();
                var display = _literalText == null ? formatter.GetDisplayForValue(Accessor, rawValue) : _literalText.ToString();
                dto.AddCellDisplay(display);

                var parameters = new RouteParameters();

                // This line of code below may be a problem later
                parameters[_idAccessor.InnerProperty.Name] = idSource().ToString();

                var url = urls.UrlFor(_inputModelType, parameters);
                dto[_linkName] = url;
            };
        }

        public IEnumerable<Accessor> SelectAccessors()
        {
            yield return _idAccessor;
            yield return Accessor;
        }

        public IEnumerable<Accessor> AllAccessors()
        {
            return SelectAccessors();
        }

        public IEnumerable<string> Headers()
        {
            yield return GetHeader();
        }

        public static LinkColumn<T> For(Expression<Func<T, object>> expression)
        {
            return new LinkColumn<T>(expression);
        }

        private void initialize()
        {
            _linkName = "linkFor" + Accessor.Name;
            IsSortable = true;
            IsFilterable = true;
        }
    }
}