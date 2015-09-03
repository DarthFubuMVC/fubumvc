using System;
using FubuMVC.RavenDb.RavenDb;
using FubuMVC.RavenDb.RavenDb.Multiple;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Document;
using Shouldly;
using StructureMap;

namespace FubuMVC.RavenDb.Tests.RavenDb.Integration
{
    [TestFixture]
    public class MultipleDatabaseConstructionTester
    {
        private Container theContainer;

        [SetUp]
        public void SetUp()
        {
            theContainer = new Container(x => {
                x.ConnectToRavenDb<SecondDbSettings>(store => {
                    store.Conventions.DefaultQueryingConsistency = ConsistencyOptions.None;
                });
                x.ConnectToRavenDb<ThirdDbSettings>(store => {
                    store.Conventions.DefaultQueryingConsistency = ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite;
                });

                x.IncludeRegistry<RavenDbRegistry>();
                x.For<RavenDbSettings>().Use(() => RavenDbSettings.InMemory());
                x.For<SecondDbSettings>().Use(new SecondDbSettings {RunInMemory = true});
                x.For<ThirdDbSettings>().Use(new ThirdDbSettings {RunInMemory = true});
            });
        }

        [TearDown]
        public void TearDown()
        {
            theContainer.Dispose();
        }

        [Test]
        public void can_create_database_store_per_type()
        {
            theContainer.GetInstance<IDocumentStore<SecondDbSettings>>()
                        .ShouldNotBeNull();

            theContainer.GetInstance<IDocumentStore<ThirdDbSettings>>()
                        .ShouldNotBeNull();
        }

        [Test]
        public void respects_the_configuration_per_store_setting_type()
        {
            theContainer.GetInstance<IDocumentStore<SecondDbSettings>>()
                        .Conventions.DefaultQueryingConsistency
                        .ShouldBe(ConsistencyOptions.None);

            theContainer.GetInstance<IDocumentStore<ThirdDbSettings>>()
                        .Conventions.DefaultQueryingConsistency
                        .ShouldBe(ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite);
        }

        [Test]
        public void document_store_is_singleton()
        {
            theContainer.GetInstance<IDocumentStore<SecondDbSettings>>()
                        .ShouldBeTheSameAs(theContainer.GetInstance<IDocumentStore<SecondDbSettings>>());

            theContainer.GetInstance<IDocumentStore<ThirdDbSettings>>()
                        .ShouldBeTheSameAs(theContainer.GetInstance<IDocumentStore<ThirdDbSettings>>());
        }

        [Test]
        public void can_build_document_session_per_type()
        {
            theContainer.GetInstance<IDocumentSession<SecondDbSettings>>()
                        .ShouldBeOfType<DocumentSession<SecondDbSettings>>();

            theContainer.GetInstance<IDocumentSession<ThirdDbSettings>>()
                        .ShouldBeOfType<DocumentSession<ThirdDbSettings>>();
        }

        [Test]
        public void default_raven_store_is_identified_as_Default()
        {
            theContainer.GetInstance<IDocumentStore>()
                        .Identifier.ShouldBe("Default");
        }

        [Test]
        public void other_raven_stores_are_identified_as_the_type()
        {
            theContainer.GetInstance<IDocumentStore<SecondDbSettings>>()
                        .Identifier.ShouldBe("SecondDbSettings");

            theContainer.GetInstance<IDocumentStore<ThirdDbSettings>>()
                        .Identifier.ShouldBe("ThirdDbSettings");
        }


        [Test]
        public void session_boundary_respects_transaction_boundaries()
        {
            var foo1 = new Foo {Id = Guid.NewGuid(), Name = "Jeremy"};
            var foo2 = new Foo {Id = foo1.Id, Name = "Josh"};

            var transaction = theContainer.GetInstance<ITransaction>();
            transaction.Execute<IDocumentSession<SecondDbSettings>>(x => x.Store(foo1));
            transaction.Execute<IDocumentSession<ThirdDbSettings>>(x => x.Store(foo2));

            transaction.Execute<IDocumentSession<SecondDbSettings>>(session => {
                session.Load<Foo>(foo1.Id).Name.ShouldBe("Jeremy");
            });

            transaction.Execute<IDocumentSession<ThirdDbSettings>>(session =>
            {
                session.Load<Foo>(foo1.Id).Name.ShouldBe("Josh");
            });

            transaction.Execute<IDocumentSession<SecondDbSettings>>(session =>
            {
                session.Load<Foo>(foo1.Id).Name.ShouldBe("Jeremy");
            });

            transaction.Execute<IDocumentSession<ThirdDbSettings>>(session =>
            {
                session.Load<Foo>(foo1.Id).Name.ShouldBe("Josh");
            });
        }
    }

    public class SecondDbSettings : RavenDbSettings
    {
        
    }

    public class ThirdDbSettings : RavenDbSettings
    {
        
    }



    public class Foo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}