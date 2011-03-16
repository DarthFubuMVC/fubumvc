using FubuMVC.Diagnostics.Models;

namespace FubuMVC.Diagnostics.Endpoints.Chains
{
	public class UnknownEndpoint
	{
		public UnknownChainRequest Get(UnknownChainRequest request)
		{
			return request;
		}
	}
}