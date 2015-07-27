using System;
using System.Linq;
using FubuMVC.RavenDb.RavenDb;
using FubuMVC.RavenDb.Tests.MultiTenancy;
using NUnit.Framework;
using Raven.Client;
using Shouldly;
using StructureMap;

namespace FubuMVC.RavenDb.Tests.RavenDb
{
    [TestFixture]
    public class RavenTransactionTester
    {
        private Container container;
        private ITransaction transaction;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            container = new Container(new RavenDbRegistry());
            container.Inject(new RavenDbSettings
            {
                RunInMemory = true
            });
        }

        [SetUp]
        public void SetUp()
        {
            transaction = container.GetInstance<ITransaction>();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            container.Dispose();
        }

        [Test]
        public void load_all()
        {
            transaction.WithRepository(repo => {
                repo.Update(new User());
                repo.Update(new User());
                repo.Update(new User());
                repo.Update(new OtherEntity());
                repo.Update(new OtherEntity());
                repo.Update(new ThirdEntity());
            });

            bool wasCalled = false;

            transaction.Execute<IDocumentSession>(session => session.Query<User>().Customize(x => x.WaitForNonStaleResults()).Any().ShouldBeTrue());
            transaction.Execute<IDocumentSession>(session => session.Query<OtherEntity>().Customize(x => x.WaitForNonStaleResults()).Any().ShouldBeTrue());
            transaction.Execute<IDocumentSession>(session => session.Query<ThirdEntity>().Customize(x => x.WaitForNonStaleResults()).Any().ShouldBeTrue());

            transaction.WithRepository(repo => {
                repo.All<User>().Count().ShouldBe(3);
                repo.All<OtherEntity>().Count().ShouldBe(2);
                repo.All<ThirdEntity>().Count().ShouldBe(1);

                wasCalled = true;
            });

            wasCalled.ShouldBeTrue();

        }

        [Test]
        public void persist()
        {
            var entity = new OtherEntity();

            transaction.WithRepository(repo => repo.Update(entity));

            transaction.Execute<IDocumentSession>(session => session.Query<OtherEntity>().Customize(x => x.WaitForNonStaleResults()).Any().ShouldBeTrue());

            bool wasCalled = false;
            transaction.WithRepository(repo => {
                repo.All<OtherEntity>().ShouldContain(entity);
                wasCalled = true;
            });

            wasCalled.ShouldBeTrue();
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


            transaction.WithRepository(tenantA, repo => {
                repo.Update(trackedA1);
                repo.Update(trackedA2);
            });

            transaction.WithRepository(tenantB, repo => {
                repo.Update(trackedB1);
                repo.Update(trackedB2);
                repo.Update(trackedB3);
            });


            transaction.Execute<IDocumentSession>(session => session.Query<TrackedEntity>().Customize(x => x.WaitForNonStaleResults()).Any().ShouldBeTrue());


            transaction.WithRepository(tenantA, repo => {
                repo.All<TrackedEntity>().ShouldHaveTheSameElementsAs(trackedA1, trackedA2);
            });

            bool wasCalled = false;
            transaction.WithRepository(tenantB, repo =>
            {
                wasCalled = true;
                repo.All<TrackedEntity>().ShouldHaveTheSameElementsAs(trackedB1, trackedB2, trackedB3);
            });

            wasCalled.ShouldBeTrue();
        }


    }
}