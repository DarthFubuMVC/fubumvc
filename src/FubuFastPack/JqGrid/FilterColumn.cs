using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Urls;

namespace FubuFastPack.JqGrid
{
    public class FilterColumn<T> : GridColumnBase<T, FilterColumn<T>>, IGridColumn
    {
        public FilterColumn(Expression<Func<T, object>> expression) : base(expression)
        {
            IsFilterable = true;
        }

        public IEnumerable<IDictionary<string, object>> ToDictionary()
        {
            yield break;
        }

        public Action<EntityDTO> CreateDtoFiller(IGridData data, IDisplayFormatter formatter, IUrlRegistry urls)
        {
            return dto => { };
        }

        public IEnumerable<Accessor> SelectAccessors()
        {
            yield break;
        }

        public IEnumerable<Accessor> AllAccessors()
        {
            yield return Accessor;
        }

        public IEnumerable<string> Headers()
        {
            yield break;
        }
    }
}