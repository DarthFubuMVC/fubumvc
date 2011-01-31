using System;

namespace FubuFastPack.Querying
{
    public class FilterTypeRegisterExpression
    {
        private readonly IFilterType _filter;

        public FilterTypeRegisterExpression(IFilterType filter)
        {
            _filter = filter;
        }

        public FilterTypeRegisterExpression ForType<T>()
        {
            FilterTypeRegistry.RegisterFilterForType(_filter, typeof(T));
            return this;
        }

        public FilterTypeRegisterExpression ForTypes(params Type[] filterableTypes)
        {
            foreach (var filterableType in filterableTypes)
            {
                FilterTypeRegistry.RegisterFilterForType(_filter, filterableType);
            }

            return this;
        }
    }
}