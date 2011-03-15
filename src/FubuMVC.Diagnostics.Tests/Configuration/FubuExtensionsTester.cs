using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Configuration;
using FubuMVC.Diagnostics.Endpoints;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Tests;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Configuration
{
    [TestFixture]
    public class FubuExtensionsTester
    {
        [Test]
        public void diagnostics_call_should_match_diagnostics_endpoint()
        {
            var endpointCall = ActionCall.For<RoutesEndpoint>(e => e.Get(new RouteRequestModel()));
            
            endpointCall
                .IsDiagnosticsCall()
                .ShouldBeTrue();
        }

        [Test]
        public void diagnostics_call_should_match_calls_with_fubu_diagnostics_url_attribute()
        {
            var callWithAttribute = ActionCall.For<DiagnosticsCallWithAttribute>(c => c.Execute());

            callWithAttribute
                .IsDiagnosticsCall()
                .ShouldBeTrue();
        }
    }
}