using FubuCore;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core.Configuration.Policies;
using FubuMVC.Diagnostics.Features;
using FubuMVC.Diagnostics.Features.Dashboard;
using FubuMVC.Diagnostics.Features.Routes;
using FubuTestingSupport;
using NUnit.Framework;
using GetHandler = FubuMVC.Diagnostics.Features.Dashboard.GetHandler;

namespace FubuMVC.Diagnostics.Tests.Configuration.Policies
{
    [TestFixture]
    public class DiagnosticsEndpointUrlPolicyTester
    {
        private DiagnosticsHandlerUrlPolicy _policy;
        private IConfigurationObserver _observer;
        [SetUp]
        public void Setup()
        {
            _policy = new DiagnosticsHandlerUrlPolicy(typeof(DiagnosticsFeatures));
            _observer = new NulloConfigurationObserver();
        }

        [Test]
        public void should_match_calls_when_handler_type_ends_with_handler_and_exists_in_diagnostics_assembly_and_no_attributes_are_found()
        {
            var endpointCall = ActionCall.For<Diagnostics.Features.Routes.GetHandler>(e => e.Execute(new DefaultRouteRequestModel()));
            _policy
                .Matches(endpointCall, _observer)
                .ShouldBeTrue();
        }

        [Test]
        public void should_not_match_calls_that_have_diagnostics_url_attribute()
        {
            var invalidCall = ActionCall.For<GetHandler>(e => e.Execute(new DashboardRequestModel()));
            _policy
                .Matches(invalidCall, _observer)
                .ShouldBeFalse();
        }

        [Test]
        public void should_not_match_calls_that_are_handlers_in_other_assemblies()
        {
            var invalidCall = ActionCall.For<InvalidHandler>(c => c.Execute());
            _policy
                .Matches(invalidCall, _observer)
                .ShouldBeFalse();
        }

        [Test]
        public void should_strip_namespace_and_make_relative_to_diagnostic_root()
        {
            var endpointCall = ActionCall.For<Diagnostics.Features.Routes.GetHandler>(e => e.Execute(new DefaultRouteRequestModel()));
            _policy
                .Build(endpointCall)
                .Pattern
                .ShouldEqual("{0}/routes".ToFormat(DiagnosticsUrls.ROOT));
        }

        public class InvalidHandler
        {
            public void Execute()
            {
            }
        }
    }
}