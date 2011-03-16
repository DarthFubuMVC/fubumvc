using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Endpoints.Routes;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Models.Routes;
using FubuMVC.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Endpoints
{
    [TestFixture]
    public class when_filtering_routes : InteractionContext<FilterEndpoint>
    {
        private BehaviorGraph _graph;
        private RouteQuery _query;
        private List<RouteDataModel> _routes;

        protected override void beforeEach()
        {
            _graph = ObjectMother.DiagnosticsGraph();
            _query = new RouteQuery();
            _routes = new List<RouteDataModel>();
            Container.Configure(x => x.For<BehaviorGraph>().Use(() => _graph));

            MockFor<IRouteDataBuilder>()
                .Expect(builder => builder.BuildRoutes(_graph))
                .Return(_routes);
        }

        [Test]
        public void should_set_total_records_from_route_data()
        {
            _routes.Add(new RouteDataModel());
            _routes.Add(new RouteDataModel());

            ClassUnderTest
                .Get(_query)
                .records
                .ShouldEqual(_routes.Count); // 2
        }

        [Test]
        public void should_set_number_of_pages_from_rows_and_route_data()
        {
            for(var i = 0; i < 100; ++i)
            {
                _routes.Add(new RouteDataModel());
            }

            _query.rows = 30;
            ClassUnderTest
                .Get(_query)
                .total
                .ShouldEqual(4);
        }

        [Test]
        public void should_set_ids_of_json_rows()
        {
            var id = Guid.NewGuid().ToString();
            _routes.Add(new RouteDataModel { Id = id });

            ClassUnderTest
                .Get(_query)
                .rows
                .ShouldContain(r => r.id.Equals(id));
        }
    }
}