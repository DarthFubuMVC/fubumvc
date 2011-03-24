using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Infrastructure;

namespace FubuMVC.Diagnostics.Grids.Columns
{
	public class AuthorizationColumn : BehaviorChainColumnBase
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

		public override bool IsIdentifier()
		{
			return false;
		}

		public override bool IsHidden(BehaviorChain chain)
		{
			return false;
		}

		public override bool HideFilter(BehaviorChain chain)
		{
			return false;
		}
	}
}