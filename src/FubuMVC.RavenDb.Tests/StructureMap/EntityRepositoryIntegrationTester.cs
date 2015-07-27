using System;
using FubuCore.Dates;
using FubuMVC.RavenDb.InMemory;
using FubuMVC.RavenDb.MultiTenancy;
using NUnit.Framework;
using Shouldly;
using StructureMap;

namespace FubuMVC.RavenDb.Tests.StructureMap
{
    [TestFixture]
    public class EntityRepositoryIntegrationTester
    {
        private IEntityRepository theRepository;
        private SimpleTenantContext theContext;
        private IPersistor thePersistor;

        [SetUp]
        public void SetUp()
        {
            var container = new Container(new InMemoryPersistenceRegistry());
            theContext = new SimpleTenantContext {CurrentTenant = Guid.NewGuid()};
            container.Inject<ITenantContext>(theContext);

            thePersistor = container.GetInstance<IPersistor>();

            theRepository = container.GetInstance<IEntityRepository>();
        }

        [Test]
        public void search_for_multi_tenanted_entities()
        {
            var e1 = new SiteEntity {Name = "Jeremy", TenantId = theContext.CurrentTenant};
            var e2 = new SiteEntity {Name = "Josh", TenantId = theContext.CurrentTenant};
            var e3 = new SiteEntity {Name = "Jeremy", TenantId = Guid.NewGuid()};
            var e4 = new SiteEntity {Name = "Josh", TenantId = Guid.NewGuid()};
        
            thePersistor.Persist(e1);
            thePersistor.Persist(e2);
            thePersistor.Persist(e3);
            thePersistor.Persist(e4);

            theRepository.All<SiteEntity>().ShouldHaveTheSameElementsAs(e1, e2);

            theRepository.FindWhere<SiteEntity>(x => x.Name == "Jeremy").ShouldBeTheSameAs(e1);

            theContext.CurrentTenant = e3.TenantId;

            theRepository.FindWhere<SiteEntity>(x => x.Name == "Jeremy").ShouldBeTheSameAs(e3);
        }

        [Test]
        public void search_for_soft_deleted_entities()
        {
            var e1 = new SoftDeletedEntity {Name = "Jeremy"};
            var e2 = new SoftDeletedEntity {Name = "Josh"};
            var e3 = new SoftDeletedEntity {Name = "Lindsey"};
            var e4 = new SoftDeletedEntity {Name = "Max"};
        
            thePersistor.Persist(e1);
            thePersistor.Persist(e2);
            thePersistor.Persist(e3);
            thePersistor.Persist(e4);

            theRepository.Remove(e3);


            e3.Deleted.ShouldNotBeNull();

            theRepository.Find<SoftDeletedEntity>(e3.Id).ShouldBeTheSameAs(e3);

            theRepository.All<SoftDeletedEntity>().ShouldHaveTheSameElementsAs(e1, e2, e4);


        }

        [Test]
        public void search_for_the_combination_of_soft_deleted_and_multi_tenancy()
        {
            var e1 = new SoftDeletedTenanted {Name = "Jeremy", TenantId = theContext.CurrentTenant, Id = Guid.NewGuid()};
            var e2 = new SoftDeletedTenanted {Name = "Jeremy", TenantId = Guid.NewGuid(), Id = Guid.NewGuid()};
            var e3 = new SoftDeletedTenanted {Name = "Josh", TenantId = theContext.CurrentTenant};
            var e4 = new SoftDeletedTenanted {Name = "Lindsey", TenantId = theContext.CurrentTenant};
            var e5 = new SoftDeletedTenanted {Name = "Max", TenantId = theContext.CurrentTenant};
        
            thePersistor.Persist(e1);
            thePersistor.Persist(e2);
            thePersistor.Persist(e3);
            thePersistor.Persist(e4);
            thePersistor.Persist(e5);

            theRepository.Remove(e1);

            theRepository.Find<SoftDeletedTenanted>(e1.Id).ShouldBeTheSameAs(e1);
            theRepository.Find<SoftDeletedTenanted>(e2.Id).ShouldBeNull(); // not the same tenant

            theRepository.FindWhere<SoftDeletedTenanted>(x => x.Name == "Jeremy").ShouldBeTheSameAs(e1);

            theRepository.All<SoftDeletedTenanted>().ShouldHaveTheSameElementsAs(e3, e4, e5);
        }
    }

    public class SiteEntity : TenantedEntity
    {
        public string Name { get; set; }
    }

    public class SoftDeletedEntity : Entity, ISoftDeletedEntity
    {
        public Milestone Deleted { get; set; }
        public string Name { get; set; }
    }

    public class SoftDeletedTenanted : TenantedEntity, ISoftDeletedEntity
    {
        public string Name { get; set; }
        public Milestone Deleted { get; set; }
    }

    public class SimpleEntity : Entity
    {
        public string Name { get; set; }
    }
}