using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Ajax;
using FubuMVC.Katana;
using FubuMVC.RavenDb.RavenDb;
using NUnit.Framework;
using Raven.Client;
using Shouldly;
using StructureMap;

namespace FubuMVC.RavenDb.Tests.RavenDb.Integration
{
    [TestFixture]
    public class TransactionBehaviorIntegratedTester
    {
        [Test]
        public void posts_are_committed()
        {
            var container = new Container(new RavenDbRegistry());
            container.Inject(new RavenDbSettings{RunInMemory = true});

            using (var application = FubuRuntime.For<NamedEntityRegistry>(_ => _.StructureMap(container)).RunEmbedded())
            {
                application.Endpoints.PostJson(new NamedEntity {Name = "Jeremy"}).StatusCode.ShouldBe(HttpStatusCode.OK);
                application.Endpoints.PostJson(new NamedEntity {Name = "Josh"}).StatusCode.ShouldBe(HttpStatusCode.OK);
                application.Endpoints.PostJson(new NamedEntity {Name = "Vyrak"}).StatusCode.ShouldBe(HttpStatusCode.OK);
            
                application.Services.Get<ITransaction>().Execute<IDocumentSession>(session => {
                    session.Query<NamedEntity>()
                           .Customize(x => x.WaitForNonStaleResults())
                           .Each(x => Debug.WriteLine(x.Name));
                });

                application.Endpoints.Get<FakeEntityEndpoint>(x => x.get_names()).ReadAsJson<NamesResponse>()
                    .Names.ShouldHaveTheSameElementsAs("Jeremy", "Josh", "Vyrak");
                    
            }
        }
    }

    public class NamedEntityRegistry : FubuRegistry
    {
        public NamedEntityRegistry()
        {
            Services.ReplaceService(new RavenDbSettings { RunInMemory = true});
            Policies.Local.Add<TransactionalBehaviorPolicy>();
        }
    }

    public class FakeEntityEndpoint
    {
        private readonly IEntityRepository _repository;

        public FakeEntityEndpoint(IEntityRepository repository)
        {
            _repository = repository;
        }

        public NamesResponse get_names()
        {
            return new NamesResponse
            {
                Names = _repository.All<NamedEntity>().OrderBy(x => x.Name).Select(x => x.Name).ToArray()
            };
        }

        public AjaxContinuation post_name(NamedEntity entity)
        {
            _repository.Update(entity);

            return AjaxContinuation.Successful();
        }
    }

    public class NamesResponse
    {
        public string[] Names { get; set; }
    }

    public class NamedEntity : Entity
    {
        public string Name { get; set; }
    }
}