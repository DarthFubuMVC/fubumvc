using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Endpoints.Routes;
using FubuMVC.Diagnostics.Grids;
using FubuMVC.Diagnostics.Models.Grids;
using FubuMVC.Tests;
using FubuTestingSupport;
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
            var query = new JsonGridQuery<BehaviorGraph>();

            MockFor<IGridService<BehaviorGraph, BehaviorChain>>()
                .Expect(s => s.GridFor(_graph, query))
                .Return(grid);

            ClassUnderTest
                .Post(query)
                .ShouldEqual(grid);
        }
    }
}