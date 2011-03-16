using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Models;

namespace FubuMVC.Diagnostics.Endpoints.Chains
{
	public class ViewEndpoint
	{
		private readonly BehaviorGraph _graph;

		public ViewEndpoint(BehaviorGraph graph)
		{
			_graph = graph;
		}

		public ChainModel Get(ChainRequest request)
		{
			var chain = _graph.Behaviors.SingleOrDefault(c => c.UniqueId == request.Id);
			if(chain == null)
			{
				throw new UnknownChainException(request.Id);
			}

			return new ChainModel {Chain = chain};
		}
	}
}