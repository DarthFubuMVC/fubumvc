using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Grids.Columns.Routes
{
	public class OriginColumn : GridColumnBase<BehaviorChain>
	{
		public OriginColumn()
			: base(chain => chain.Origin)
		{
		}

		public override bool IsHidden(BehaviorChain target)
		{
			return true;
		}
	}
}