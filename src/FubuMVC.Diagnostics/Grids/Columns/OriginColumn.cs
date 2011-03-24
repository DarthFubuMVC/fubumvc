using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Grids.Columns
{
	public class OriginColumn : BehaviorChainColumnBase
	{
		public OriginColumn()
			: base(chain => chain.Origin)
		{
		}

		public override string ValueFor(BehaviorChain chain)
		{
			return chain.Origin;
		}

		public override bool IsIdentifier()
		{
			return false;
		}

		public override bool IsHidden(BehaviorChain chain)
		{
			return true;
		}

		public override bool HideFilter(BehaviorChain chain)
		{
			return false;
		}
	}
}