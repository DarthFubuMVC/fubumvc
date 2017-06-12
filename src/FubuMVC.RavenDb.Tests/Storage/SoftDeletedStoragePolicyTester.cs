using FubuCore.Dates;
using FubuMVC.RavenDb.Storage;
using Shouldly;
using Xunit;

namespace FubuMVC.RavenDb.Tests.Storage
{
    public class SoftDeletedStoragePolicyTester
    {
        private SoftDeletedStoragePolicy thePolicy;

        public  SoftDeletedStoragePolicyTester()
        {
            thePolicy = new SoftDeletedStoragePolicy(SystemTime.Default());
        }

        [Fact]
        public void does_not_match_a_regular_entity()
        {
            thePolicy.Matches<RegularEntity>().ShouldBeFalse();
        }

        [Fact]
        public void matches_a_soft_deleted_entity()
        {
            thePolicy.Matches<SoftDeletedEntity>().ShouldBeTrue();
        }

        [Fact]
        public void create_the_wrapper()
        {
            var inner = new EntityStorage<SoftDeletedEntity>(null);
            thePolicy.Wrap(inner).ShouldBeOfType<SoftDeletedEntityStorage<SoftDeletedEntity>>()
                .Inner.ShouldBeTheSameAs(inner);
        }
    }

    public class RegularEntity : Entity{}
}
