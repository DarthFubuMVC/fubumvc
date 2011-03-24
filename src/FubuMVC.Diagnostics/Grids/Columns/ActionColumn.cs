using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Grids.Columns
{
	public class ActionColumn : BehaviorChainColumnBase
	{
		public ActionColumn()
			: base("Action")
		{
		}

		public override string ValueFor(BehaviorChain chain)
		{
			return chain.FirstCallDescription;
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