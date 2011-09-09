using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core.Infrastructure;

namespace FubuMVC.Diagnostics.Features.Chains.View
{
	public class get_Id_handler
	{
		private readonly BehaviorGraph _graph;
	    private readonly IHttpConstraintResolver _constraintResolver;

		public get_Id_handler(BehaviorGraph graph, IHttpConstraintResolver constraintResolver)
		{
		    _graph = graph;
		    _constraintResolver = constraintResolver;
		}

	    public ChainModel Execute(ChainRequest request)
		{
			var chain = _graph.Behaviors.SingleOrDefault(c => c.UniqueId == request.Id);
			if(chain == null)
			{
                throw new ArgumentException("{0} does not exist".ToFormat(request.Id));
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