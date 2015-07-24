using FubuMVC.Core;
using FubuMVC.Core.Security.Authentication.Auditing;
using FubuMVC.RavenDb.InMemory;
using FubuMVC.RavenDb.Membership;
using Shouldly;
using NUnit.Framework;
using StructureMap;

namespace FubuPersistence.Tests.Membership
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
                    FubuApplication.For<FubuRepoWithPersistedMembership>(_ => _.StructureMap(container)).Bootstrap())
            {
                application.Factory.Get<ILoginAuditor>().ShouldBeOfType<PersistedLoginAuditor>();
            }
        }
    }
}