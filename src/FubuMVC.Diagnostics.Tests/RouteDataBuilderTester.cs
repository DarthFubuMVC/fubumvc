using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Tests;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests
{
    [TestFixture]
    public class RouteDataBuilderTester
    {
        private BehaviorGraph _graph;
        private BehaviorChain _chain;
        private RouteDataBuilder _builder;
        private IEnumerable<RouteDataModel> _models;

        [SetUp]
        public void Setup()
        {
            _graph = ObjectMother.DiagnosticsGraph();
            _chain = _graph.BehaviorFor(typeof (DashboardRequestModel));
            _builder = new RouteDataBuilder();
            _models = _builder.BuildRoutes(_graph);
        }

        [Test]
        public void should_set_id_from_chain_id()
        {
            _models
                .ShouldContain(m => m.Id == _chain.UniqueId.ToString());
        }

        [Test]
        public void should_set_input_model()
        {
            _models
                .ShouldContain(m => m.InputModel == typeof(DashboardRequestModel).Name);
        }

        [Test]
        public void should_set_output_model()
        {
            _models
                .ShouldContain(m => m.OutputModel == typeof(DashboardModel).Name);
        }

        [Test]
        public void should_set_action_description()
        {
            _models
                .ShouldContain(m => m.Action == _chain.FirstCallDescription);
        }

        [Test]
        public void should_set_constraints()
        {
            _models
                .ShouldContain(m => m.Constraints == "GET");
        }
    }
}