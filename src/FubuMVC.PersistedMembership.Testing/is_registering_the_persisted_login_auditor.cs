using FubuMVC.Authentication;
using FubuMVC.Authentication.Auditing;
using FubuMVC.Core;
using FubuPersistence.InMemory;
using NUnit.Framework;
using FubuMVC.StructureMap;
using StructureMap;
using FubuTestingSupport;

namespace FubuMVC.PersistedMembership.Testing
{


    [TestFixture]
    public class is_registering_the_persisted_login_auditor
    {
        [Test]
        public void PersistedLoginAuditor_is_registered()
        {
            var container = new Container(new InMemoryPersistenceRegistry());
            using (var application = FubuApplication.For<FubuRepoWithPersistedMembership>().StructureMap(container).Bootstrap())
            {
                application.Factory.Get<ILoginAuditor>().ShouldBeOfType<PersistedLoginAuditor>();
            }
            
        }
    }
}