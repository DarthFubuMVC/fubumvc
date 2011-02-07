using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuFastPack.Domain;
using FubuFastPack.Querying;
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
    public class LinkColumn<T> : GridColumnBase<T>, IGridColumn where T : DomainEntity
    {
        private readonly Accessor _idAccessor;
        private string _linkName;

        public static LinkColumn<T> For(Expression<Func<T, object>> expression)
        {
            return new LinkColumn<T>(expression);
        }

        public LinkColumn(Expression<Func<T, object>> expression) : base(expression)
        {
            _idAccessor = ReflectionHelper.GetAccessor<T>(x => x.Id);
            _linkName = "linkFor" + Accessor.Name;

            IsSortable = true;
        }

        // TODO -- UT this little monster
        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>{
                {"name", Accessor.Name},
                {"index", Accessor.Name},
                {"sortable", IsSortable},
                {"linkName", _linkName},
                {"formatter", "link"}
            };
        }

        public Action<EntityDTO> CreateFiller(IGridData data, IDisplayFormatter formatter, IUrlRegistry urls)
        {
            var displaySource = data.GetterFor(Accessor);
            var idSource = data.GetterFor(_idAccessor);

            return dto =>
            {
                var display = formatter.GetDisplay(Accessor, displaySource());
                dto.AddCellDisplay(display);

                var parameters = new RouteParameters();
                parameters[_idAccessor.Name] = idSource().ToString();

                var url = urls.UrlFor<T>(parameters);
                dto[_linkName] = url;
            };
        }

        public IEnumerable<Accessor> SelectAccessors()
        {
            yield return Accessor;
        }

        public IEnumerable<Accessor> AllAccessors()
        {
            return SelectAccessors();
        }
    }
}