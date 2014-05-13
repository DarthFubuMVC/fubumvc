using FubuMVC.Core;
using FubuMVC.Diagnostics.Dashboard;
using FubuMVC.Diagnostics.Model;
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
                runtime.Behaviors.BehaviorFor<GroupEndpoint>(x => x.Group(null)).Authorization.AllowedRoles().ShouldContain("admin");
                runtime.Behaviors.BehaviorFor<DashboardFubuDiagnostics>(x => x.Index(null)).Authorization.AllowedRoles().ShouldContain("admin");
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