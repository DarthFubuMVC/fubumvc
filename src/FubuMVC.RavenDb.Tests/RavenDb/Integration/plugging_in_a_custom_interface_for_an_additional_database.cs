using System;
using FubuMVC.RavenDb.RavenDb;
using FubuMVC.RavenDb.RavenDb.Multiple;
using Raven.Client.Document;
using Shouldly;
using StructureMap;
using Xunit;

namespace FubuMVC.RavenDb.Tests.RavenDb.Integration
{
    public class plugging_in_a_custom_interface_for_an_additional_database : IDisposable
    {
        private Container theContainer;

        public plugging_in_a_custom_interface_for_an_additional_database()
        {
            theContainer = new Container(x =>
            {
                x.ConnectToRavenDb<FourthDbSettings>(store =>
                {
                    store.Conventions.DefaultQueryingConsistency = ConsistencyOptions.None;
                }).Using<IFourthDatabase, FourthDatabase>();


                x.IncludeRegistry<RavenDbRegistry>();
                x.For<RavenDbSettings>().Use(() => RavenDbSettings.InMemory());
                x.For<FourthDbSettings>().Use(new FourthDbSettings { RunInMemory = true });
            });
        }

        [Fact]
        public void session_boundary_respects_transaction_boundaries()
        {
            var foo1 = new Foo { Id = Guid.NewGuid(), Name = "Jeremy" };

            var transaction = theContainer.GetInstance<ITransaction>();
            transaction.Execute<IFourthDatabase>(x => x.Store(foo1));

            transaction.Execute<IFourthDatabase>(session =>
            {
                session.Load<Foo>(foo1.Id).Name.ShouldBe("Jeremy");
            });

            transaction.Execute<IFourthDatabase>(session =>
            {
                session.Load<Foo>(foo1.Id).Name.ShouldBe("Jeremy");
            });


        }

        public void Dispose()
        {
            theContainer?.Dispose();
        }
    }

    public class FourthDbSettings : RavenDbSettings
    {

    }

    public interface IFourthDatabase : IDocumentSession<FourthDbSettings> { }

    public class FourthDatabase : DocumentSession<FourthDbSettings>, IFourthDatabase
    {
        public FourthDatabase(ISessionBoundary boundary) : base(boundary)
        {
        }
    }
}
