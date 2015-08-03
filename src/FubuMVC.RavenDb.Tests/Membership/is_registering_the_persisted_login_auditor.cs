using FubuMVC.Core;
using FubuMVC.Core.Security.Authentication.Auditing;
using FubuMVC.RavenDb.InMemory;
using FubuMVC.RavenDb.Membership;
using NUnit.Framework;
using Shouldly;
using StructureMap;

namespace FubuMVC.RavenDb.Tests.Membership
{
    [TestFixture]
    public class is_registering_the_persisted_login_auditor
    {
        [Test]
        public void PersistedLoginAuditor_is_registered()
        {
            var container = new Container(new InMemoryPersistenceRegistry());
            using (
                var application =
                    FubuRuntime.For<FubuRepoWithPersistedMembership>(_ => _.StructureMap(container)))
            {
                application.Get<ILoginAuditor>().ShouldBeOfType<PersistedLoginAuditor>();
            }
        }
    }
}