using FubuMVC.Diagnostics.Core.Grids;
using FubuMVC.Diagnostics.Features.Requests;
using FubuMVC.Diagnostics.Features.Requests.Filter;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Models.Grids;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Features.Requests
{
	[TestFixture]
	public class when_filtering_requests : InteractionContext<PostHandler>
	{
		// sanity check
		[Test]
		public void should_build_grid_from_debug_report()
		{
			var model = new RequestCacheModel();
			var query = new JsonGridQuery<RequestCacheModel>();
			var grid = new JsonGridModel();

			MockFor<IRequestCacheModelBuilder>()
				.Expect(b => b.Build())
				.Return(model);

			MockFor<IGridService<RequestCacheModel, RecordedRequestModel>>()
				.Expect(s => s.GridFor(model, query))
				.Return(grid);

			ClassUnderTest
				.Execute(query)
				.ShouldEqual(grid);
		}
	}
}