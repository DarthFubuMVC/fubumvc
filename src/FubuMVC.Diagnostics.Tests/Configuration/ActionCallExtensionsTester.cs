using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core.Configuration;
using FubuMVC.Diagnostics.Features.Routes;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Configuration
{
    [TestFixture]
    public class ActionCallExtensionsTester
    {
        [Test]
        public void diagnostics_call_should_match_diagnostics_endpoint()
        {
            var endpointCall = ActionCall.For<GetHandler>(e => e.Execute(new DefaultRouteRequestModel()));
            
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