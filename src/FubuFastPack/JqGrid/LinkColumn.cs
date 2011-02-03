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

    public class LinkColumn<T> : GridColumnBase<T>, IGridColumn where T : DomainEntity
    {
        private readonly Accessor _idAccessor;

        public static LinkColumn<T> For(Expression<Func<T, object>> expression)
        {
            return new LinkColumn<T>(expression.ToAccessor(), expression);
        }

        public LinkColumn(Accessor accessor, Expression<Func<T, object>> expression) : base(accessor, expression)
        {
            _idAccessor = ReflectionHelper.GetAccessor<T>(x => x.Id);
        }

        public IDictionary<string, object> ToDictionary()
        {
            throw new NotImplementedException();
        }

        public Action<EntityDTO> CreateFiller(IGridData data, IDisplayFormatter formatter, IUrlRegistry urls)
        {
            var displaySource = data.GetterFor(Accessor);
            var idSource = data.GetterFor(_idAccessor);

            return dto =>
            {
                var display = formatter.GetDisplay(Accessor, displaySource());
                dto.AddCellDisplay(display);

                throw new NotImplementedException("NEED TO PLAY WITH UI FIRST");
            };
        }

        public IEnumerable<Accessor> SelectAccessors()
        {
            yield return Accessor;
        }
    }
}