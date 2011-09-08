using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Core.Grids.Columns.Routes
{
	public class InputModelColumn : GridColumnBase<BehaviorChain>
	{
		public InputModelColumn()
			: base("InputModel")
		{
		}

		public override int Rank()
		{
			return 2;
		}

		public override string ValueFor(BehaviorChain chain)
		{
			return chain.InputType() == null ? string.Empty : chain.InputType().FullName;
		}
	}
}