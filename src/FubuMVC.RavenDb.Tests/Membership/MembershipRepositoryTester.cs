using FubuMVC.Core.Security.Authentication;
using FubuMVC.RavenDb.Membership;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.RavenDb.Tests.Membership
{
    [TestFixture]
    public class MembershipRepositoryTester
    {
        private EntityRepository theRepository;
        private PasswordHash theHash;
        private MembershipRepository<FubuMVC.RavenDb.Membership.User> theMembership;

        [SetUp]
        public void SetUp()
        {
            theRepository = EntityRepository.InMemory();
            theHash = new PasswordHash();

            theMembership = new MembershipRepository<FubuMVC.RavenDb.Membership.User>(theRepository, theHash);
        }

        [Test]
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

        [Test]
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

        [Test]
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