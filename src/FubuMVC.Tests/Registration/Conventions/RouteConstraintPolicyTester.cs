using System.Web.Routing;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class when_applying_a_constraint_policy_with_http_method_filters
    {
        private RouteConstraintPolicy _policy;
        private IRouteDefinition _routeDefinition;
        private SpecificationExtensions.CapturingConstraint _argsToAddConstraint;
        private IConfigurationObserver _observer;

        [SetUp]
        public void Setup()
        {
            _policy = new RouteConstraintPolicy();
            _routeDefinition = MockRepository.GenerateMock<IRouteDefinition>();
            _observer = MockRepository.GenerateMock<IConfigurationObserver>();
            _policy.AddHttpMethodFilter(x => x.Method.Name.StartsWith("Query"), "GET");
            _policy.AddHttpMethodFilter(x => x.Method.Name.EndsWith("Command"), "POST");

            _argsToAddConstraint = _routeDefinition.CaptureArgumentsFor(r => r.AddRouteConstraint(null, null));
        }


        [Test]
        public void should_add_an_HttpMethodConstraint_to_the_route_definition_for_the_method_that_applies()
        {
            _policy.Apply(ActionCall.For<SampleForConstraintPolicy>(c => c.QueryParts()), _routeDefinition, _observer);

            _argsToAddConstraint.First<string>().ShouldEqual(RouteConstraintPolicy.HTTP_METHOD_CONSTRAINT);
            _argsToAddConstraint.Second<IRouteConstraint>().ShouldBeOfType<HttpMethodConstraint>()
                .AllowedMethods.ShouldHaveTheSameElementsAs("GET");
        }

        [Test]
        public void should_add_an_HttpMethodConstraint_to_the_route_definition_for_multiple_methods_that_appli()
        {
            _policy.Apply(ActionCall.For<SampleForConstraintPolicy>(c => c.QueryPartsAndAddCommand()), _routeDefinition, _observer);

            _argsToAddConstraint.First<string>().ShouldEqual(RouteConstraintPolicy.HTTP_METHOD_CONSTRAINT);
            _argsToAddConstraint.Second<IRouteConstraint>().ShouldBeOfType<HttpMethodConstraint>()
                .AllowedMethods.ShouldHaveTheSameElementsAs("GET", "POST");
        }

        [Test]
        public void should_log_when_adding_constraints()
        {
            _policy.Apply(ActionCall.For<SampleForConstraintPolicy>(c => c.QueryParts()), _routeDefinition, _observer);

            _observer.AssertWasCalled(o => o.RecordCallModification(null, null), o=>o.IgnoreArguments());
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