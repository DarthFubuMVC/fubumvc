using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Grids.Columns.Routes
{
	public class OutputModelColumn : GridColumnBase<BehaviorChain>
	{
		public OutputModelColumn()
			: base("OutputModel")
		{
		}

		public override string ValueFor(BehaviorChain chain)
		{
			return chain.ActionOutputType() == null ? string.Empty : chain.ActionOutputType().FullName;
		}
	}
}