using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Configuration.SparkPolicies;
using FubuMVC.Diagnostics.Tests.Endpoints;
using FubuMVC.Diagnostics.Tests.Endpoints.Nested;
using FubuMVC.Tests;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Configuration.Spark
{
    [TestFixture]
    public class DiagnosticsEndpointSparkPolicyTester
    {
        private DiagnosticsEndpointSparkPolicy _policy;

        [SetUp]
        public void Setup()
        {
            _policy = new DiagnosticsEndpointSparkPolicy(typeof(TestEndpoint));
        }

        [Test]
        public void should_match_diagnostic_calls()
        {
            var diagnosticsCall = ActionCall.For<DiagnosticsCallWithAttribute>(call => call.Execute());
            
            _policy
                .Matches(diagnosticsCall)
                .ShouldBeTrue();
        }

        [Test]
        public void should_build_view_locator_off_root_endpoint_name()
        {
            var rootEndpointCall = ActionCall.For<TestEndpoint>(e => e.Execute());
            _policy
                .BuildViewLocator(rootEndpointCall)
                .ShouldBeEmpty();
        }

        [Test]
        public void should_build_view_locator_for_nested_endpoints()
        {
            var nestedEndpoint = ActionCall.For<AnotherEndpoint>(e => e.Execute());
            _policy
                .BuildViewLocator(nestedEndpoint)
                .ShouldEqual("Nested");
        }

        [Test]
        public void should_build_view_name_from_endpoint_name()
        {
            var endpoint = ActionCall.For<TestEndpoint>(e => e.Execute());
            _policy
                .BuildViewName(endpoint)
                .ShouldEqual("Test");
        }
    }
}