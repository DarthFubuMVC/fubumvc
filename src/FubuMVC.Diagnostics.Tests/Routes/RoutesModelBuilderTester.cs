using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core.Grids;
using FubuMVC.Diagnostics.Features.Routes;
using FubuMVC.Diagnostics.Models.Grids;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Routes
{
    [TestFixture]
    public class RoutesModelBuilderTester : InteractionContext<RoutesModelBuilder>
    {
        private List<JsonGridColumn> _columns;
        private BehaviorChain _firstChain;
        private BehaviorGraph _graph;

        protected override void beforeEach()
        {
            _columns = new List<JsonGridColumn>
                           {
                               new JsonGridColumn { Name = "Test" }
                           };

            _graph = new FubuRegistry(r => r.IncludeDiagnostics(true)).BuildGraph();
            Container.Inject(_graph);

            _firstChain = _graph.Behaviors.First();
        }

        [Test]
        public void should_build_columns_from_policies()
        {
            MockFor<IGridColumnBuilder<BehaviorChain>>()
                .Expect(builder => builder.ColumnsFor(_firstChain))
                .Return(_columns);

            ClassUnderTest
                .Build()
                .ColumnModel
                .Columns
                .ShouldContain(c => c.name.Equals("Test"));
        }
    }
}