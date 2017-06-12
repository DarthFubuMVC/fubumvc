using FubuMVC.Core.Security.Authentication;
using FubuMVC.RavenDb.Membership;
using Shouldly;
using Xunit;

namespace FubuMVC.RavenDb.Tests.Membership
{
    public class MembershipRepositoryTester
    {
        private EntityRepository theRepository;
        private PasswordHash theHash;
        private MembershipRepository<FubuMVC.RavenDb.Membership.User> theMembership;

        public MembershipRepositoryTester()
        {
            theRepository = EntityRepository.InMemory();
            theHash = new PasswordHash();

            theMembership = new MembershipRepository<FubuMVC.RavenDb.Membership.User>(theRepository, theHash);
        }

        [Fact]
        public void matches_credentials_postive()
        {
            var user1 = new FubuMVC.RavenDb.Membership.User
            {
                UserName = "jeremy",
                Password = theHash.CreateHash("something")
            };

            var user2 = new FubuMVC.RavenDb.Membership.User
            {
                UserName = "josh",
                Password = theHash.CreateHash("else")
            };

            theRepository.Update(user1);
            theRepository.Update(user2);


            theMembership.MatchesCredentials(new LoginRequest
            {
                UserName = "jeremy",
                Password = "wrong"
            }).ShouldBeFalse();

            theMembership.MatchesCredentials(new LoginRequest
            {
                UserName = "jeremy",
                Password = "else"
            }).ShouldBeFalse();

            theMembership.MatchesCredentials(new LoginRequest
            {
                UserName = "josh",
                Password = "something"
            }).ShouldBeFalse();
        }

        [Fact]
        public void matches_credentials_negative()
        {
            var user1 = new FubuMVC.RavenDb.Membership.User
            {
                UserName = "jeremy",
                Password = theHash.CreateHash("something")
            };

            var user2 = new FubuMVC.RavenDb.Membership.User
            {
                UserName = "josh",
                Password = theHash.CreateHash("else")
            };

            theRepository.Update(user1);
            theRepository.Update(user2);


            theMembership.MatchesCredentials(new LoginRequest
            {
                UserName = "jeremy",
                Password = "something"
            }).ShouldBeTrue();

            theMembership.MatchesCredentials(new LoginRequest
            {
                UserName = "josh",
                Password = "else"
            }).ShouldBeTrue();
        }

        [Fact]
        public void find_by_name()
        {
            var user1 = new FubuMVC.RavenDb.Membership.User
            {
                UserName = "jeremy",
                Password = theHash.CreateHash("something")
            };

            var user2 = new FubuMVC.RavenDb.Membership.User
            {
                UserName = "josh",
                Password = theHash.CreateHash("else")
            };

            theRepository.Update(user1);
            theRepository.Update(user2);

            theMembership.FindByName("jeremy").ShouldBeTheSameAs(user1);
            theMembership.FindByName("josh").ShouldBeTheSameAs(user2);
        }
    }
}
