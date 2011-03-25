using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Endpoints.Routes;
using FubuMVC.Diagnostics.Grids;
using FubuMVC.Diagnostics.Infrastructure;
using FubuMVC.Diagnostics.Models.Grids;
using FubuMVC.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Endpoints
{
    [TestFixture]
    public class when_filtering_routes : InteractionContext<FilterEndpoint>
    {
        private BehaviorGraph _graph;

        protected override void beforeEach()
        {
            _graph = ObjectMother.DiagnosticsGraph();
            Container.Configure(x => x.For<BehaviorGraph>().Use(() => _graph));
        }

        [Test]
        public void should_build_grid_from_grid_service()
        {
            var grid = new JsonGridModel();
            var gridQuery = new JsonGridQuery();
			var query = new RouteQuery();

        	MockFor<IJsonProvider>()
        		.Expect(p => p.Deserialize<JsonGridQuery>(query.Body))
        		.Return(gridQuery);

			JsonService.Stub(MockFor<IJsonProvider>());

            MockFor<IGridService<BehaviorGraph, BehaviorChain>>()
                .Expect(s => s.GridFor(_graph, gridQuery))
                .Return(grid);

            ClassUnderTest
                .Post(query)
                .ShouldEqual(grid);
        }
    }
}