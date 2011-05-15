using System.Linq;
using System.Web.Routing;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class when_applying_a_constraint_policy_with_http_method_filters
    {
        private RouteConstraintPolicy _policy;
        private IRouteDefinition _routeDefinition;
        private RecordingConfigurationObserver _observer;

        [SetUp]
        public void Setup()
        {
            _policy = new RouteConstraintPolicy();
            _routeDefinition = new RouteDefinition("something");
            _observer = new RecordingConfigurationObserver();
            _policy.AddHttpMethodFilter(x => x.Method.Name.StartsWith("Query"), "GET");
            _policy.AddHttpMethodFilter(x => x.Method.Name.EndsWith("Command"), "POST");

        }


        [Test]
        public void should_add_an_HttpMethodConstraint_to_the_route_definition_for_the_method_that_applies()
        {
            _policy.Apply(ActionCall.For<SampleForConstraintPolicy>(c => c.QueryParts()), _routeDefinition, _observer);

            _routeDefinition.AllowedHttpMethods.ShouldHaveTheSameElementsAs("GET");
        }

        [Test]
        public void should_add_an_HttpMethodConstraint_to_the_route_definition_for_multiple_methods_that_apply()
        {
            _policy.Apply(ActionCall.For<SampleForConstraintPolicy>(c => c.QueryPartsAndAddCommand()), _routeDefinition,
                          _observer);

            _routeDefinition.AllowedHttpMethods.ShouldHaveTheSameElementsAs("GET", "POST");
        }

        [Test]
        public void should_log_for_each_method_that_applies()
        {
            var call = ActionCall.For<SampleForConstraintPolicy>(c => c.QueryPartsAndAddCommand());
            _policy.Apply(call, _routeDefinition, _observer);

            var log = _observer.GetLog(call);

            log.First().ShouldContain("GET");
            log.Skip(1).First().ShouldContain("POST");
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