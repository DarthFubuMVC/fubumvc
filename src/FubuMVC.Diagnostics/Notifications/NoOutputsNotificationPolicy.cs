using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Grids.Columns;
using FubuMVC.Diagnostics.Grids.Filters;
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
			var filter = new JsonGridFilter {ColumnName = _viewFilter.Column.Name(), Values = new[] {ViewColumn.None}};
			return _graph
				.Behaviors
				.Where(chain => _viewFilter.AppliesTo(chain, filter)
				                && _viewFilter.Matches(chain, filter));
		}
	}
}