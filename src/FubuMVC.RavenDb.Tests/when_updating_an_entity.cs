using System;
using System.Configuration;
using FubuMVC.RavenDb.Storage;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.RavenDb.Tests
{
    [TestFixture]
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

        [Test]
        public void should_update()
        {
            ClassUnderTest.Update(theEntity);
            theStorage.AssertWasCalled(x => x.Update(theEntity));
        }

        public static void Try()
        {
            ConfigurationManager.AppSettings.Set("Raven/Port", "8081");

            ConfigurationManager.AppSettings.Get("Raven/Port").ShouldBe("8081");
        }

    }
}