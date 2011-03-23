using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Infrastructure.Grids.Filters
{
    public class HttpMethodFilter : IGridFilter<BehaviorChain>
    {
        public const string Constraints = "Constraints";
        private readonly IHttpConstraintResolver _constraintResolver;

        public HttpMethodFilter(IHttpConstraintResolver constraintResolver)
        {
            _constraintResolver = constraintResolver;
        }

        public bool AppliesTo(BehaviorChain target, JsonGridFilter filter)
        {
            return filter.ColumnName.Equals(Constraints, StringComparison.OrdinalIgnoreCase);
        }

        public bool Matches(BehaviorChain target, JsonGridFilter filter)
        {
            return filter.Matches(_constraintResolver.Resolve(target),
                                  (value, constraints) => constraints.Contains(value));
        }
    }
}