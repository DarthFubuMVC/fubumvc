using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Urls;
using FubuMVC.Diagnostics.Models;

namespace FubuMVC.Diagnostics.Grids.Columns
{
	public class ChainUrlColumn : BehaviorChainColumnBase
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

		public override bool IsIdentifier()
		{
			return true;
		}

		public override bool IsHidden(BehaviorChain chain)
		{
			return true;
		}

		public override bool HideFilter(BehaviorChain chain)
		{
			return true;
		}
	}
}