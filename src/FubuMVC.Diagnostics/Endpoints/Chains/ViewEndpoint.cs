using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core;
using FubuMVC.Diagnostics.Core.Infrastructure;
using FubuMVC.Diagnostics.Models;

namespace FubuMVC.Diagnostics.Endpoints.Chains
{
	public class ViewEndpoint
	{
		private readonly BehaviorGraph _graph;
	    private readonly IHttpConstraintResolver _constraintResolver;

		public ViewEndpoint(BehaviorGraph graph, IHttpConstraintResolver constraintResolver)
		{
		    _graph = graph;
		    _constraintResolver = constraintResolver;
		}

	    public ChainModel Get(ChainRequest request)
		{
			var chain = _graph.Behaviors.SingleOrDefault(c => c.UniqueId == request.Id);
			if(chain == null)
			{
				throw new UnknownObjectException(request.Id);
			}

		    return new ChainModel
		               {
		                   Chain = chain,
                           Constraints = _constraintResolver.Resolve(chain),
		                   Behaviors = chain.Select(x =>
		                                                {
		                                                    var behavior = new BehaviorModel {BehaviorType = x.ToString()};
		                                                    var call = x as ActionCall;
                                                            if(call != null)
                                                            {
                                                                behavior.Logs = _graph.Observer.GetLog(call);
                                                            }

		                                                    return behavior;
		                                                })
			           };
		}
	}
}