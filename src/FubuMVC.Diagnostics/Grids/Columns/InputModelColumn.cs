using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Grids.Columns
{
	public class InputModelColumn : BehaviorChainColumnBase
	{
		public InputModelColumn()
			: base("InputModel")
		{
		}

		public override string ValueFor(BehaviorChain chain)
		{
			return chain.InputType() == null ? string.Empty : chain.InputType().FullName;
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