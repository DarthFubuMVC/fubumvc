using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Infrastructure.Grids.Filters
{
    public class ViewFilter : IGridFilter<BehaviorChain>
    {
		public const string View = "View";

        public bool AppliesTo(BehaviorChain target, JsonGridFilter filter)
        {
			return filter.ColumnName.Equals(View, StringComparison.OrdinalIgnoreCase);
        }

        public bool Matches(BehaviorChain target, JsonGridFilter filter)
        {
            return filter.Matches(target.Outputs.Select(output => output.Description).Join(","), (value, output) => output.Contains(value));
        }
    }
}