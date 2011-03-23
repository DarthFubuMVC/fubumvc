using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Infrastructure.Grids.Filters
{
    public class UrlCategoryFilter : IGridFilter<BehaviorChain>
    {
        public const string UrlCategory = "UrlCategory";

        public bool AppliesTo(BehaviorChain target, JsonGridFilter filter)
        {
            return filter.ColumnName.Equals(UrlCategory, StringComparison.OrdinalIgnoreCase);
        }

        public bool Matches(BehaviorChain target, JsonGridFilter filter)
        {
            return filter.Matches(target.UrlCategory.Category, (value, category) => category.Contains(value));
        }
    }
}