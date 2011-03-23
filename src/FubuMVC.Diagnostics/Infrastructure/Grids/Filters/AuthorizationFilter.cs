using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Infrastructure.Grids.Filters
{
    public class AuthorizationFilter : IGridFilter<BehaviorChain>
    {
        public const string Authorization = "Authorization";
        private readonly IAuthorizationDescriptor _descriptor;

        public AuthorizationFilter(IAuthorizationDescriptor descriptor)
        {
            _descriptor = descriptor;
        }

        public bool AppliesTo(BehaviorChain target, JsonGridFilter filter)
        {
            return filter.ColumnName.Equals(Authorization, StringComparison.OrdinalIgnoreCase);
        }

        public bool Matches(BehaviorChain target, JsonGridFilter filter)
        {
            return filter.Matches(_descriptor.AuthorizorFor(target).RulesDescriptions().Join(", "),
                                  (value, policies) => policies.Contains(value));
        }
    }
}