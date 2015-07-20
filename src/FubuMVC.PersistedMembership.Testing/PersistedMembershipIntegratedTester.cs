using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Membership;
using FubuPersistence.InMemory;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.PersistedMembership.Testing
{
    [TestFixture]
    public class PersistedMembershipIntegratedTester
    {
        [Test]
        public void build_application_with_persisted_membership()
        {
            using (var runtime = FubuApplication
                .For<FubuRepoWithPersistedMembership>(_ => _.StructureMap<InMemoryPersistenceRegistry>())
                .Bootstrap())
            {
                var container = runtime.Factory.Get<IContainer>();

                container.GetInstance<IMembershipRepository>()
                    .ShouldBeOfType<MembershipRepository<User>>();

                container.GetInstance<IPasswordHash>().ShouldBeOfType<PasswordHash>();

                container.GetAllInstances<IAuthenticationStrategy>()
                    .OfType<MembershipAuthentication>()
                    .Any(x => x.Membership is MembershipRepository<User>).ShouldBeTrue();
            }
        }
    }

    public class FubuRepoWithPersistedMembership : FubuRegistry
    {
        public FubuRepoWithPersistedMembership()
        {
            Import<PersistedMembership<User>>();
        }
    }
}