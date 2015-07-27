using FubuCore.Dates;
using FubuMVC.RavenDb.Storage;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.RavenDb.Tests.Storage
{
    [TestFixture]
    public class SoftDeletedStoragePolicyTester
    {
        private SoftDeletedStoragePolicy thePolicy;

        [SetUp]
        public void SetUp()
        {
            thePolicy = new SoftDeletedStoragePolicy(SystemTime.Default());
        }

        [Test]
        public void does_not_match_a_regular_entity()
        {
            thePolicy.Matches<RegularEntity>().ShouldBeFalse();
        }

        [Test]
        public void matches_a_soft_deleted_entity()
        {
            thePolicy.Matches<SoftDeletedEntity>().ShouldBeTrue();
        }

        [Test]
        public void create_the_wrapper()
        {
            var inner = new EntityStorage<SoftDeletedEntity>(null);
            thePolicy.Wrap(inner).ShouldBeOfType<SoftDeletedEntityStorage<SoftDeletedEntity>>()
                .Inner.ShouldBeTheSameAs(inner);
        }
    }

    public class RegularEntity : Entity{}
}