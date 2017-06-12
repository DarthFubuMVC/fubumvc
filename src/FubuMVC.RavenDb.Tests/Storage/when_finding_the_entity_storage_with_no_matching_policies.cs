using FubuMVC.RavenDb.Storage;
using FubuMVC.Tests.TestSupport;
using Rhino.Mocks;
using Shouldly;
using Xunit;

namespace FubuMVC.RavenDb.Tests.Storage
{
    public class when_finding_the_entity_storage_with_no_matching_policies : InteractionContext<StorageFactory>
    {
        protected override void beforeEach()
        {
            MockFor<IEntityStoragePolicy>().Stub(x => x.Matches<User>()).Return(false);
        }

        [Fact]
        public void uses_the_global_entity_storage_by_default()
        {
            ClassUnderTest.StorageFor<User>().ShouldBeOfType<EntityStorage<User>>();
        }
    }
}
