using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core.Infrastructure;

namespace FubuMVC.Diagnostics.Core.Grids.Columns.Routes
{
	public class AuthorizationColumn : GridColumnBase<BehaviorChain>
	{
		private readonly IAuthorizationDescriptor _descriptor;

		public AuthorizationColumn(IAuthorizationDescriptor descriptor)
			: base("Authorization")
		{
			_descriptor = descriptor;
		}

		public override string ValueFor(BehaviorChain chain)
		{
			var rules = _descriptor.AuthorizorFor(chain).RulesDescriptions();
			return rules.Any() ? rules.Join(", ") : "None";
		}

		public override bool IsHidden(BehaviorChain target)
		{
			return true;
		}
	}
}