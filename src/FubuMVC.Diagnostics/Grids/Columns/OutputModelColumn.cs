using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Grids.Columns
{
	public class OutputModelColumn : BehaviorChainColumnBase
	{
		public OutputModelColumn()
			: base("OutputModel")
		{
		}

		public override string ValueFor(BehaviorChain chain)
		{
			return chain.ActionOutputType() == null ? string.Empty : chain.ActionOutputType().FullName;
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