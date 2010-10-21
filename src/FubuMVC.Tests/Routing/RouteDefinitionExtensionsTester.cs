using System.Web.Routing;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Routes;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Routing
{
    [TestFixture]
    public class RouteDefinitionExtensionsTester
    {
        private IRouteDefinition _routeDefinition;
        private SpecificationExtensions.CapturingConstraint _argsToAddConstraint;

        [SetUp]
        public void Setup()
        {
            _routeDefinition = MockRepository.GenerateMock<IRouteDefinition>();
            _argsToAddConstraint = _routeDefinition.CaptureArgumentsFor(r => r.AddRouteConstraint(null, null));
        }

        [Test]
        public void should_add_an_HttpMethodConstraint_to_the_route_definition_for_the_specified_methods()
        {
            _routeDefinition.ConstrainToHttpMethods("GET", "POST");

            _argsToAddConstraint
                .First<string>()
                .ShouldEqual(RouteConstraintPolicy.HTTP_METHOD_CONSTRAINT);

            _argsToAddConstraint
                .Second<IRouteConstraint>()
                .ShouldBeOfType<HttpMethodConstraint>()
                .AllowedMethods
                .ShouldHaveTheSameElementsAs("GET", "POST");
        }
    }
}