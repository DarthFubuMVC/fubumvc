using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Grids.Columns.Routes
{
	public class ActionColumn : GridColumnBase<BehaviorChain>
	{
		public ActionColumn()
			: base("Action")
		{
		}

		public override string ValueFor(BehaviorChain chain)
		{
			return chain.FirstCallDescription;
		}
	}
}