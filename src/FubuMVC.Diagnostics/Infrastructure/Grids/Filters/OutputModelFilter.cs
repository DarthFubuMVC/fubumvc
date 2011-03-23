using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Infrastructure.Grids.Filters
{
    public class OutputModelFilter : IGridFilter<BehaviorChain>
    {
        public const string OutputModel = "OutputModel";

        public bool AppliesTo(BehaviorChain target, JsonGridFilter filter)
        {
            return filter.ColumnName.Equals(OutputModel, StringComparison.OrdinalIgnoreCase);
        }

        public bool Matches(BehaviorChain target, JsonGridFilter filter)
        {
            if (target.ActionOutputType() == null)
            {
                return false;
            }

            return filter.Matches(target.ActionOutputType().FullName, (value, type) => type.Contains(value));
        }
    }
}