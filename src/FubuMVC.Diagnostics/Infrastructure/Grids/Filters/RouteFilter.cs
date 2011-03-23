using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Infrastructure.Grids.Filters
{
    public class RouteFilter : IGridFilter<BehaviorChain>
    {
        public const string Route = "Route";

        public bool AppliesTo(BehaviorChain target, JsonGridFilter filter)
        {
            return filter.ColumnName.Equals(Route, StringComparison.OrdinalIgnoreCase);
        }

        public bool Matches(BehaviorChain target, JsonGridFilter filter)
        {
            return filter.Matches(target.RoutePattern, (value, route) => route.StartsWith(value));
        }
    }
}