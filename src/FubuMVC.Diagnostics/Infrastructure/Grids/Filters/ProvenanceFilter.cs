using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Infrastructure.Grids.Filters
{
    public class ProvenanceFilter : IGridFilter<BehaviorChain>
    {
        public const string Provenance = "Provenance";

        public bool AppliesTo(BehaviorChain target, JsonGridFilter filter)
        {
            return filter.ColumnName.Equals(Provenance, StringComparison.OrdinalIgnoreCase);
        }

        public bool Matches(BehaviorChain target, JsonGridFilter filter)
        {
            return filter.Matches(target.Origin, (value, origin) => origin.Contains(value));
        }
    }
}