using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Endpoints;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class apply_authorization_rights_to_all_diagnostics_pages
    {
        [Test]
        public void should_be_applied_to_all()
        {
            FubuMode.SetUpForDevelopmentMode();
            using (var runtime = FubuApplication.For<AuthorizedRegistry>().Bootstrap())
            {
                runtime.Behaviors.BehaviorFor<FubuDiagnosticsEndpoint>(x => x.get__fubu()).Authorization.AllowedRoles().ShouldContain("admin");
                runtime.Behaviors.BehaviorFor<EndpointExplorerFubuDiagnostics>(x => x.get_endpoints()).Authorization.AllowedRoles().ShouldContain("admin");
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