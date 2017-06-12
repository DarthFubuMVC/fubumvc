using System;
using FubuMVC.RavenDb.Storage;
using FubuMVC.Tests.TestSupport;
using Rhino.Mocks;
using Xunit;

namespace FubuMVC.RavenDb.Tests
{
    public class when_updating_an_entity : InteractionContext<EntityRepository>
    {
        private FakeEntity theEntity;
        private IEntityStorage<FakeEntity> theStorage;

        protected override void beforeEach()
        {
            theEntity = new FakeEntity();

            theStorage = MockFor<IEntityStorage<FakeEntity>>();
            MockFor<IStorageFactory>().Stub(x => x.StorageFor<FakeEntity>()).Return(
                theStorage);

            LocalSystemTime = DateTime.Now;
        }

        [Fact]
        public void should_update()
        {
            ClassUnderTest.Update(theEntity);
            theStorage.AssertWasCalled(x => x.Update(theEntity));
        }
    }
}
