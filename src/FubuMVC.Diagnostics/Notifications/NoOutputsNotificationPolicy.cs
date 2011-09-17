using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core.Grids.Columns.Routes;
using FubuMVC.Diagnostics.Core.Grids.Filters.Routes;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Notifications
{
	public class NoOutputsNotificationPolicy : INotificationPolicy
	{
		private readonly BehaviorGraph _graph;
		private readonly ViewFilter _viewFilter;

		public NoOutputsNotificationPolicy(BehaviorGraph graph, ViewFilter viewFilter)
		{
			_graph = graph;
			_viewFilter = viewFilter;
		}

		public bool Applies()
		{
			return getChainsWithoutOutput().Any();
		}

		public INotificationModel Build()
		{
			var chains = getChainsWithoutOutput();
			return new NoOutputsNotification
			       	{
						BehaviorCount = chains.Count(),
						Filter = new FilterLink(_viewFilter.Column.Name(), ViewColumn.None)
			       	};
		}

		private IEnumerable<BehaviorChain> getChainsWithoutOutput()
		{
		    return _graph
		        .Behaviors
		        .Where(chain =>
		                   {
		                       var call = chain.LastCall();
		                       return !chain.Outputs.Any() && (call == null || !call.OutputType().Equals(typeof (FubuContinuation)));
		                   });
		}
	}
}