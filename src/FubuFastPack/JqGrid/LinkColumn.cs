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
        private readonly IList<Action<IDictionary<string, object>>> _alterations = new List<Action<IDictionary<string, object>>>();

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

        private Action<IDictionary<string, object>> alter
        {
            set
            {
                _alterations.Add(value);
            }
        }

        public LinkColumn<T> Formatter(string columnFormatName)
        {
            alter = dict => dict["formatter"] = columnFormatName;

            return this;
        }

        public Accessor IdAccessor
        {
            get { return _idAccessor; }
        }

        public Type InputModelType
        {
            get { return _inputModelType; }
        }

        public string LinkName
        {
            get { return _linkName; }
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

            _alterations.Each(a => a(dictionary));

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

        public virtual Action<EntityDTO> CreateDtoFiller(IGridData data, IDisplayFormatter formatter, IUrlRegistry urls)
        {
            var displaySource = data.GetterFor(Accessor);
            var idSource = data.GetterFor(IdAccessor);
            var urlSource = this.toUrlSource(urls, idSource);

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

                dto[_linkName] = urlSource();
            };
        }

        protected virtual Func<string> toUrlSource(IUrlRegistry urls, Func<object> idSource)
        {
            return () =>
            {
                var parameters = new RouteParameters();
                parameters[_idAccessor.InnerProperty.Name] = idSource().ToString();
                return urls.UrlFor(_inputModelType, parameters);
            };
        }

        public IEnumerable<Accessor> SelectAccessors()
        {
            yield return Accessor;
            yield return _idAccessor;
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

        public LinkColumn<T> TrimToLength(int length)
        {
            alter = dict =>
            {
                dict["formatter"] = "trimmedLink";
                dict["trim-length"] = length;
            };

            return this;
        }
    }
}