using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Endpoints;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class apply_authorization_rights_to_all_diagnostics_pages
    {
        [Test]
        public void should_be_applied_to_all()
        {
            using (var runtime = FubuRuntime.For<AuthorizedRegistry>(_ => _.Mode = "development"))
            {
                runtime.Behaviors.ChainFor<FubuDiagnosticsEndpoint>(x => x.get__fubu()).Authorization.AllowedRoles().ShouldContain("admin");
                runtime.Behaviors.ChainFor<EndpointExplorerFubuDiagnostics>(x => x.get_endpoints()).Authorization.AllowedRoles().ShouldContain("admin");
            }
        }
    }

    public class AuthorizedRegistry : FubuRegistry
    {
        public AuthorizedRegistry()
        {
            AlterSettings<DiagnosticsSettings>(x => x.RestrictToRole("admin"));
        }
    }
}