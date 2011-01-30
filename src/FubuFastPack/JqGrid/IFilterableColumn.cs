using System.Linq;
using FubuCore.Reflection;
using FubuFastPack.Querying;
using FubuLocalization;

namespace FubuFastPack.JqGrid
{
    public interface IFilterableColumn
    {
        // TODO -- may need to have other services here in the 
        // signature
        FilterDTO ToDTO();
    }

    public class FilterableColumn : IFilterableColumn
    {
        private readonly Accessor _accessor;

        public FilterableColumn(Accessor accessor)
        {
            _accessor = accessor;
        }

        public FilterDTO ToDTO()
        {
            return new FilterDTO{
                display = LocalizationManager.GetHeader(_accessor.InnerProperty),
                value = _accessor.Name,
                operators = FilterTypeRegistry.GetFiltersFor(_accessor.PropertyType).Select(x => x.ToDTO()).ToArray()
            };
        }
    }
}