using System.Linq;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class when_applying_a_constraint_policy_with_http_method_filters
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            _policy = new RouteConstraintPolicy();
            _routeDefinition = new RouteDefinition("something");
            _observer = new RecordingConfigurationObserver();
            _policy.AddHttpMethodFilter(x => x.Method.Name.StartsWith("Query"), "GET");
            _policy.AddHttpMethodFilter(x => x.Method.Name.EndsWith("Command"), "POST");
        }

        #endregion

        private RouteConstraintPolicy _policy;
        private IRouteDefinition _routeDefinition;
        private RecordingConfigurationObserver _observer;

        [Test]
        public void should_add_an_HttpMethodConstraint_to_the_route_definition_for_multiple_methods_that_apply()
        {
            _policy.Apply(ActionCall.For<SampleForConstraintPolicy>(c => c.QueryPartsAndAddCommand()), _routeDefinition,
                          _observer);

            _routeDefinition.AllowedHttpMethods.ShouldHaveTheSameElementsAs("GET", "POST");
        }

        [Test]
        public void should_add_an_HttpMethodConstraint_to_the_route_definition_for_the_method_that_applies()
        {
            _policy.Apply(ActionCall.For<SampleForConstraintPolicy>(c => c.QueryParts()), _routeDefinition, _observer);

            _routeDefinition.AllowedHttpMethods.ShouldHaveTheSameElementsAs("GET");
        }

    }

    public class SampleForConstraintPolicy
    {
        public void QueryParts()
        {
        }

        public void AddPartCommand()
        {
        }

        public void QueryPartsAndAddCommand()
        {
        }
    }
}