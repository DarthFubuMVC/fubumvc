using System.Linq;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;

namespace FubuMVC.Authentication
{
	[Title("Any action with the [PassThroughAuthentication] attribute")]
	public class IncludePassThroughAuthentication : IChainFilter
	{
		public bool Matches(BehaviorChain chain)
		{
			return chain.Calls.Any(IsPassThrough);
		}

		public static bool IsPassThrough(ActionCall call)
		{
			if (call.HasAttribute<PassThroughAuthenticationAttribute>()) return true;

			if (call.InputType() != null && call.InputType().HasAttribute<PassThroughAuthenticationAttribute>())
			{
				return true;
			}

			return false;
		}
	}

	[Title("Any action with the [PassThroughAuthentication] attribute")]
	public class ExcludePassThroughAuthentication : IChainFilter
	{
		public bool Matches(BehaviorChain chain)
		{
			return chain.Calls.Any(IncludePassThroughAuthentication.IsPassThrough);
		}
	}
}