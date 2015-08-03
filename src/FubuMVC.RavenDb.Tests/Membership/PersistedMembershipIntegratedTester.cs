using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Membership;
using FubuMVC.RavenDb.InMemory;
using FubuMVC.RavenDb.Membership;
using NUnit.Framework;
using Shouldly;
using StructureMap;

namespace FubuMVC.RavenDb.Tests.Membership
{
    [TestFixture]
    public class PersistedMembershipIntegratedTester
    {
        [Test]
        public void build_application_with_persisted_membership()
        {
            using (var runtime = FubuRuntime
                .For<FubuRepoWithPersistedMembership>(_ =>
                {
                    _.Features.Authentication.Enable(true);
                    _.Services.IncludeRegistry<InMemoryPersistenceRegistry>();
                })
                )
            {
                var container = runtime.Get<IContainer>();

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