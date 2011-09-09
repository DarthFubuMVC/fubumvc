using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core.Grids;
using FubuMVC.Diagnostics.Features.Routes.Filter;
using FubuMVC.Diagnostics.Models.Grids;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Routes
{
    [TestFixture]
    public class when_filtering_routes : InteractionContext<PostHandler>
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
                .Execute(query)
                .ShouldEqual(grid);
        }
    }
}