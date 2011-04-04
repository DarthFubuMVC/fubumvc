using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Urls;
using FubuMVC.Diagnostics.Models;

namespace FubuMVC.Diagnostics.Grids.Columns.Routes
{
	public class ChainUrlColumn : GridColumnBase<BehaviorChain>
	{
		private readonly IUrlRegistry _urls;

		public ChainUrlColumn(IUrlRegistry urls)
			: base("ChainUrl")
		{
			_urls = urls;
		}

		public override string ValueFor(BehaviorChain chain)
		{
			return _urls.UrlFor(new ChainRequest {Id = chain.UniqueId});
		}

		public override bool IsHidden(BehaviorChain chain)
		{
			return true;
		}

		public override bool HideFilter(BehaviorChain target)
		{
			return true;
		}

		public override bool IsIdentifier()
		{
			return true;
		}
	}
}