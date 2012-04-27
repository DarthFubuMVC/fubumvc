using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Core.Grids.Columns.Routes
{
	public class OutputModelColumn : GridColumnBase<BehaviorChain>
	{
		public OutputModelColumn()
			: base("OutputModel")
		{
		}

		public override int Rank()
		{
			return 1;
		}

		public override string ValueFor(BehaviorChain chain)
		{
			return chain.ResourceType() == null ? string.Empty : chain.ResourceType().FullName;
		}
	}
}