using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Grids.Columns.Routes
{
	public class UrlCategoryColumn : GridColumnBase<BehaviorChain>
	{
		public UrlCategoryColumn()
			: base(chain => chain.UrlCategory)
		{
		}

		public override string ValueFor(BehaviorChain chain)
		{
			return chain.UrlCategory.Category;
		}

		public override bool IsHidden(BehaviorChain chain)
		{
			return true;
		}
	}
}