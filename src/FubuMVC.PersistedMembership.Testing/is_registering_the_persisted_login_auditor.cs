using FubuMVC.Core;
using FubuMVC.Core.Security.Authentication.Auditing;
using FubuPersistence.InMemory;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.PersistedMembership.Testing
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