using FubuCore;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Membership;
using FubuMVC.Core.Security.Authentication.Membership.FlatFile;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Security.Authentication.Membership.FlatFile
{
    [TestFixture]
    public class FlatFileMembershipRepositoryTester
    {
        private FlatFileMembershipRepository theRepository;

        [SetUp]
        public void SetUp()
        {
            var files = FubuApplicationFiles.ForDefault();

            theRepository = new FlatFileMembershipRepository(files);
        }

        [TearDown]
        public void Teardown()
        {
            new FileSystem().DeleteFile(theRepository.PasswordConfigFile);
        }

        [Test]
        public void write_and_read()
        {
            var user1 = new UserInfo
            {
                UserName = "Jeremy",
                Roles = new string[] {"A", "B"}
            };

            var user2 = new UserInfo
            {
                UserName = "Josh",
                Roles = new string[] {"C", "D"}
            };

            theRepository.Write(new UserInfo[]{user1, user2});

            theRepository.FindByName("Josh").ShouldBeTheSameAs(user2);
            theRepository.FindByName("Jeremy").ShouldBeTheSameAs(user1);
        }

        [Test]
        public void read_cleanly_from_a_persisted_file()
        {
            var user1 = new UserInfo
            {
                UserName = "Jeremy",
                Roles = new string[] { "A", "B" }
            };

            var user2 = new UserInfo
            {
                UserName = "Josh",
                Roles = new string[] { "C", "D" }
            };

            theRepository.Write(new UserInfo[] { user1, user2 });

            var secondRepository = new FlatFileMembershipRepository(FubuApplicationFiles.ForDefault());

            secondRepository.FindByName("Josh").ShouldNotBeNull();
            secondRepository.FindByName("Josh").Roles.ShouldHaveTheSameElementsAs("C", "D");

        }

        [Test]
        public void matches_credentials_positive()
        {
            var user1 = new UserInfo
            {
                UserName = "Jeremy",
                Password = "alba",
                Roles = new string[] { "A", "B" }
            };

            var user2 = new UserInfo
            {
                UserName = "Josh",
                Roles = new string[] { "C", "D" }
            };

            theRepository.Write(new UserInfo[] { user1, user2 });

            theRepository.MatchesCredentials(new LoginRequest
            {
                UserName = "Jeremy",
                Password = "alba"
            }).ShouldBeTrue();
        }

        [Test]
        public void matches_credentials_negative()
        {
            var user1 = new UserInfo
            {
                UserName = "Jeremy",
                Password = "alba",
                Roles = new string[] { "A", "B" }
            };

            var user2 = new UserInfo
            {
                UserName = "Josh",
                Roles = new string[] { "C", "D" }
            };

            theRepository.Write(new UserInfo[] { user1, user2 });

            theRepository.MatchesCredentials(new LoginRequest
            {
                UserName = "Jeremy",
                Password = "wrong"
            }).ShouldBeFalse();

            theRepository.MatchesCredentials(new LoginRequest
            {
                UserName = "Wrong",
                Password = "alba"
            }).ShouldBeFalse();
        }
    }
}