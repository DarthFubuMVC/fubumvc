using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Membership;
using FubuMVC.RavenDb.Membership;
using FubuPersistence.InMemory;
using Shouldly;
using NUnit.Framework;
using StructureMap;

namespace FubuPersistence.Tests.Membership
{
    [TestFixture]
    public class PersistedMembershipIntegratedTester
    {
        [Test]
        public void build_application_with_persisted_membership()
        {
            using (var runtime = FubuApplication
                .For<FubuRepoWithPersistedMembership>(_ =>
                {
                    _.Features.Authentication.Enable(true);
                    _.StructureMap<InMemoryPersistenceRegistry>();
                })
                .Bootstrap())
            {
                var container = runtime.Factory.Get<IContainer>();

                container.GetInstance<IMembershipRepository>()
                    .ShouldBeOfType<MembershipRepository<FubuMVC.RavenDb.Membership.User>>();

                container.GetInstance<IPasswordHash>().ShouldBeOfType<PasswordHash>();

                container.GetAllInstances<IAuthenticationStrategy>()
                    .OfType<MembershipAuthentication>()
                    .Any(x => x.Membership is MembershipRepository<FubuMVC.RavenDb.Membership.User>).ShouldBeTrue();
            }
        }
    }

    public class FubuRepoWithPersistedMembership : FubuRegistry
    {
        public FubuRepoWithPersistedMembership()
        {
            Import<PersistedMembership<FubuMVC.RavenDb.Membership.User>>();
        }
    }
}