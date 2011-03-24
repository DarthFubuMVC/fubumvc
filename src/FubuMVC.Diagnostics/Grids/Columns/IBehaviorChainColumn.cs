using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Grids.Columns
{
	public interface IBehaviorChainColumn
	{
		string Name();
		string ValueFor(BehaviorChain chain);
		bool IsIdentifier();
		bool IsHidden(BehaviorChain chain);
		bool HideFilter(BehaviorChain chain);
	}
}