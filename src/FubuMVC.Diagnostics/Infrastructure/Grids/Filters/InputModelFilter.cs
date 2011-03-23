using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Infrastructure.Grids.Filters
{
    public class InputModelFilter : IGridFilter<BehaviorChain>
    {
        public const string InputModel = "InputModel";

        public bool AppliesTo(BehaviorChain target, JsonGridFilter filter)
        {
            return filter.ColumnName.Equals(InputModel, StringComparison.OrdinalIgnoreCase);
        }

        public bool Matches(BehaviorChain target, JsonGridFilter filter)
        {
            if(target.InputType() == null)
            {
                return false;
            }

            return filter.Matches(target.InputType().Name, (value, type) => type.Contains(value));
        }
    }
}