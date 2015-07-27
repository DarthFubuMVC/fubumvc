using System;
using FubuCore.Dates;
using FubuMVC.RavenDb.InMemory;
using FubuMVC.RavenDb.Storage;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.RavenDb.Tests.Storage
{
    [TestFixture]
    public class SoftDeletedEntityStorageTester : InteractionContext<SoftDeletedEntityStorage<SoftDeletedEntity>>
    {
        private InMemoryPersistor thePersistor;

        protected override void beforeEach()
        {
            LocalSystemTime = DateTime.Today.AddHours(8);


            thePersistor = new InMemoryPersistor();
            Services.Inject<IEntityStorage<SoftDeletedEntity>>(new EntityStorage<SoftDeletedEntity>(thePersistor));
        }


        [Test]
        public void can_still_find_a_soft_deleted_object()
        {
            var @case = new SoftDeletedEntity();
            @case.Deleted.ShouldBeNull();

            ClassUnderTest.Remove(@case);


            ClassUnderTest.Find(@case.Id).Id.ShouldBe(@case.Id);
        }

        [Test]
        public void can_still_find_where_can_find_a_soft_deleted_object()
        {
            var @case = new SoftDeletedEntity();
            @case.Deleted.ShouldBeNull();

            ClassUnderTest.Remove(@case);

            ClassUnderTest.FindSingle(c => c.Id == @case.Id).Id.ShouldBe(@case.Id);
        }

        [Test]
        public void soft_deleted_entities_are_not_available_from_All()
        {
            var c1 = new SoftDeletedEntity();
            var c2 = new SoftDeletedEntity {Deleted = new Milestone(DateTime.Now)};
            var c3 = new SoftDeletedEntity();
            var c4 = new SoftDeletedEntity {Deleted = new Milestone(DateTime.Now)};

            thePersistor.Persist(c1);
            thePersistor.Persist(c2);
            thePersistor.Persist(c3);
            thePersistor.Persist(c4);

            thePersistor.Persist(c2);
            thePersistor.Persist(c4);

            ClassUnderTest.All().ShouldHaveTheSameElementsAs(c1, c3);
        }

        [Test]
        public void when_deleting_an_entity_mark_the_entity_as_deleted()
        {
            var @case = new SoftDeletedEntity();
            @case.Deleted.ShouldBeNull();

            ClassUnderTest.Remove(@case);

            @case.Deleted.ShouldBe(new Milestone(LocalSystemTime));
        }
    }


    public class SoftDeletedEntity : ISoftDeletedEntity
    {
        #region ISoftDeletedEntity Members

        public Guid Id { get; set; }
        public Milestone Deleted { get; set; }

        #endregion
    }
}