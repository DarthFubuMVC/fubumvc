using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Grids.Columns
{
	public class UrlCategoryColumn : BehaviorChainColumnBase
	{
		public UrlCategoryColumn()
			: base(chain => chain.UrlCategory)
		{
		}

		public override string ValueFor(BehaviorChain chain)
		{
			return chain.UrlCategory.Category;
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