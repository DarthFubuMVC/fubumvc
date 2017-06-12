using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.RavenDb.MultiTenancy;
using FubuMVC.RavenDb.RavenDb;
using Raven.Client;
using Shouldly;
using StructureMap;
using Xunit;

namespace FubuMVC.RavenDb.Tests.RavenDb.Integration
{
    public class TransactionBehaviorIntegratedTester
    {
        [Fact]
        public void posts_are_committed()
        {
            var container = new Container(new RavenDbRegistry());
            container.Configure(x => x.For<ITenantContext>().Use<NulloTenantContext>());

            container.Inject(new RavenDbSettings{RunInMemory = true});

            using (var application = FubuRuntime.For<NamedEntityRegistry>(_ =>
            {
                _.StructureMap(container);
                _.HostWith<Katana>();
            }))
            {
                application.Scenario(_ =>
                {
                    _.Post.Json(new NamedEntity {Name = "Jeremy"});
                });

                application.Scenario(_ =>
                {
                    _.Post.Json(new NamedEntity { Name = "Josh" });
                });

                application.Scenario(_ =>
                {
                    _.Post.Json(new NamedEntity { Name = "Vyrak" });
                });

                application.Get<ITransaction>().Execute<IDocumentSession>(session => {
                    session.Query<NamedEntity>()
                           .Customize(x => x.WaitForNonStaleResults())
                           .Each(x => Debug.WriteLine(x.Name));
                });

                application.Scenario(_ =>
                {
                    _.Get.Action<FakeEntityEndpoint>(x => x.get_names());
                    _.Response.Body.ReadAsJson<NamesResponse>()
                        .Names.ShouldHaveTheSameElementsAs("Jeremy", "Josh", "Vyrak");
                });
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
