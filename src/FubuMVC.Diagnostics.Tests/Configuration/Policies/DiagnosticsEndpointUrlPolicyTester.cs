using FubuCore;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Configuration.Policies;
using FubuMVC.Diagnostics.Endpoints;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Models.Routes;
using FubuMVC.Tests;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Configuration.Policies
{
    [TestFixture]
    public class DiagnosticsEndpointUrlPolicyTester
    {
        private DiagnosticsEndpointUrlPolicy _policy;
        private IConfigurationObserver _observer;
        [SetUp]
        public void Setup()
        {
            _policy = new DiagnosticsEndpointUrlPolicy();
            _observer = new NulloConfigurationObserver();
        }

        [Test]
        public void should_match_calls_when_handler_type_ends_with_endpoint_and_exists_in_diagnostics_assembly_and_no_attributes_are_found()
        {
            var endpointCall = ActionCall.For<RoutesEndpoint>(e => e.Get(new RouteRequestModel()));
            _policy
                .Matches(endpointCall, _observer)
                .ShouldBeTrue();
        }

        [Test]
        public void should_not_match_calls_that_are_not_endpoints()
        {
            var invalidCall = ActionCall.For<InvalidClass>(c => c.Execute());
            _policy
                .Matches(invalidCall, _observer)
                .ShouldBeFalse();
        }

        [Test]
        public void should_not_match_calls_that_have_diagnostics_url_attribute()
        {
            var invalidCall = ActionCall.For<DashboardEndpoint>(e => e.Get(new DashboardRequestModel()));
            _policy
                .Matches(invalidCall, _observer)
                .ShouldBeFalse();
        }

        [Test]
        public void should_not_match_calls_that_are_endpoints_in_other_assemblies()
        {
            var invalidCall = ActionCall.For<InvalidEndpoint>(c => c.Execute());
            _policy
                .Matches(invalidCall, _observer)
                .ShouldBeFalse();
        }

        [Test]
        public void should_strip_namespace_and_make_relative_to_diagnostic_root()
        {
            var endpointCall = ActionCall.For<RoutesEndpoint>(e => e.Get(new RouteRequestModel()));
            _policy
                .Build(endpointCall)
                .Pattern
                .ShouldEqual("{0}/routes".ToFormat(DiagnosticsUrls.ROOT));
        }

        public class InvalidEndpoint
        {
            public void Execute()
            {
            }
        }

        public class InvalidClass
        {
            public void Execute()
            {
            }
        }
    }
}