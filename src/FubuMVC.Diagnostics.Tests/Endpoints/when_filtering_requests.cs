using FubuMVC.Core.Diagnostics;
using FubuMVC.Diagnostics.Endpoints.Requests;
using FubuMVC.Diagnostics.Grids;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Models.Grids;
using FubuMVC.Diagnostics.Models.Requests;
using FubuMVC.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Endpoints
{
	[TestFixture]
	public class when_filtering_requests : InteractionContext<FilterEndpoint>
	{
		// sanity check
		[Test]
		public void should_build_grid_from_debug_report()
		{
			var model = new DebugReportModel();
			var query = new JsonGridQuery<DebugReportModel>();
			var grid = new JsonGridModel();

			MockFor<IDebugReportModelBuilder>()
				.Expect(b => b.Build(MockFor<IDebugReport>()))
				.Return(model);

			MockFor<IGridService<DebugReportModel, BehaviorReportModel>>()
				.Expect(s => s.GridFor(model, query))
				.Return(grid);

			ClassUnderTest
				.Post(query)
				.ShouldEqual(grid);
		}
	}
}