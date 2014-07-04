using FubuMVC.Core;
using FubuMVC.Diagnostics.Endpoints;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests
{
    [TestFixture]
    public class apply_authorization_rights_to_all_diagnostics_pages
    {
        [Test]
        public void should_be_applied_to_all()
        {
            using (var runtime = FubuApplication.For<AuthorizedRegistry>().StructureMap().Bootstrap())
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
            AlterSettings<DiagnosticsSettings>(x => x.RestrictToRule("admin"));
        }
    }
}