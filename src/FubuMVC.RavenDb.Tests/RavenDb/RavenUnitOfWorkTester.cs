using System;
using System.Linq;
using FubuMVC.RavenDb.RavenDb;
using FubuMVC.RavenDb.Tests.MultiTenancy;
using NUnit.Framework;
using Shouldly;
using StructureMap;

namespace FubuMVC.RavenDb.Tests.RavenDb
{
    [TestFixture]
    public class RavenUnitOfWorkTester
    {
        private Container container;
        private RavenUnitOfWork theUnitOfWork;

        [SetUp]
        public void SetUp()
        {
            container = new Container(new RavenDbRegistry());
            container.Inject(new RavenDbSettings{RunInMemory = true});

            theUnitOfWork = new RavenUnitOfWork(container);
        }

        [TearDown]
        public void TearDown()
        {
            container.Dispose();
        }

        [Test]
        public void starting_the_unit_of_work_twice_throws_an_exception()
        {
            theUnitOfWork.Start();

            Exception<InvalidOperationException>.ShouldBeThrownBy(() => {
                theUnitOfWork.Start();
            });
        }


        [Test]
        public void starting_the_unit_of_work_twice_throws_an_exception_2()
        {
            theUnitOfWork.Start();

            Exception<InvalidOperationException>.ShouldBeThrownBy(() =>
            {
                theUnitOfWork.Start(Guid.NewGuid());
            });
        }

        [Test]
        public void starting_the_unit_of_work_twice_throws_an_exception_3()
        {
            theUnitOfWork.Start(Guid.NewGuid());

            Exception<InvalidOperationException>.ShouldBeThrownBy(() =>
            {
                theUnitOfWork.Start(Guid.NewGuid());
            });
        }

        [Test]
        public void starting_the_unit_of_work_twice_throws_an_exception_4()
        {
            theUnitOfWork.Start(Guid.NewGuid());

            Exception<InvalidOperationException>.ShouldBeThrownBy(() =>
            {
                theUnitOfWork.Start();
            });
        }

        [Test]
        public void commit_and_load_all()
        {
            var repo = theUnitOfWork.Start();

            repo.Update(new User());
            repo.Update(new User());
            repo.Update(new User());
            repo.Update(new OtherEntity());
            repo.Update(new OtherEntity());
            repo.Update(new ThirdEntity());

            theUnitOfWork.Commit();

            repo = theUnitOfWork.Start();

            repo.All<User>().Count().ShouldBe(3);
            repo.All<OtherEntity>().Count().ShouldBe(2);
            repo.All<ThirdEntity>().Count().ShouldBe(1);
        }

        [Test]
        public void reject_saves_nothing()
        {
            var repo = theUnitOfWork.Start();

            repo.Update(new User());
            repo.Update(new User());
            repo.Update(new User());
            repo.Update(new OtherEntity());
            repo.Update(new OtherEntity());
            repo.Update(new ThirdEntity());

            theUnitOfWork.Reject();

            repo = theUnitOfWork.Start();

            repo.All<User>().Count().ShouldBe(0);
            repo.All<OtherEntity>().Count().ShouldBe(0);
            repo.All<ThirdEntity>().Count().ShouldBe(0);
        }

        [Test]
        public void multi_tenancy_test_to_excercise_the_service_arguments()
        {
            var tenantA = Guid.NewGuid();
            var tenantB = Guid.NewGuid();


            var trackedA1 = new TrackedEntity();
            var trackedA2 = new TrackedEntity();
            var trackedB1 = new TrackedEntity();
            var trackedB2 = new TrackedEntity();
            var trackedB3 = new TrackedEntity();

            var repo = theUnitOfWork.Start(tenantA);
            repo.Update(trackedA1);
            repo.Update(trackedA2);
            theUnitOfWork.Commit();

            repo = theUnitOfWork.Start(tenantB);
            repo.Update(trackedB1);
            repo.Update(trackedB2);
            repo.Update(trackedB3);
            theUnitOfWork.Commit();

            theUnitOfWork.Start(tenantA)
                .All<TrackedEntity>().ShouldHaveTheSameElementsAs(trackedA1, trackedA2);
            theUnitOfWork.Reject();

            theUnitOfWork.Start(tenantB)
                .All<TrackedEntity>().ShouldHaveTheSameElementsAs(trackedB1, trackedB2, trackedB3);
            theUnitOfWork.Reject();
        }
    }
}