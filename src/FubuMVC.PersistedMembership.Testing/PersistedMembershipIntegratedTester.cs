using FubuMVC.Authentication;
using FubuMVC.Authentication.Membership;
using FubuMVC.Core;
using FubuMVC.Core.StructureMap;
using FubuPersistence.InMemory;
using NUnit.Framework;
using StructureMap;
using FubuTestingSupport;
using System.Linq;

namespace FubuMVC.PersistedMembership.Testing
{
    [TestFixture]
    public class PersistedMembershipIntegratedTester
    {
        [Test]
        public void build_application_with_persisted_membership()
        {
            var container = new Container(new InMemoryPersistenceRegistry());

            using (var runtime = FubuApplication
                .For<FubuRepoWithPersistedMembership>()
                .StructureMap(container)
                .Bootstrap())
            {
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