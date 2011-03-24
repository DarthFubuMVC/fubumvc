using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Infrastructure;

namespace FubuMVC.Diagnostics.Grids.Columns
{
	public class ConstraintsColumn : BehaviorChainColumnBase
	{
		private readonly IHttpConstraintResolver _resolver;
		
		public ConstraintsColumn(IHttpConstraintResolver resolver)
			: base("Constraints")
		{
			_resolver = resolver;
		}

		public override string ValueFor(BehaviorChain chain)
		{
			return _resolver.Resolve(chain);
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