using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuMVC.RavenDb.InMemory;
using FubuMVC.RavenDb.MultiTenancy;
using FubuMVC.RavenDb.Storage;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.RavenDb.Tests.MultiTenancy
{
    [TestFixture]
    public class ByTenantEntityStorageTester
    {
        private EntityStorage<TrackedEntity> inner;
        private SimpleTenantContext theContext;
        private ByTenantEntityStorage<TrackedEntity> theStorage;

        [SetUp]
        public void SetUp()
        {
            inner = new EntityStorage<TrackedEntity>(new InMemoryPersistor());
            theContext = new SimpleTenantContext
            {
                CurrentTenant = Guid.NewGuid()
            };

            theStorage = new ByTenantEntityStorage<TrackedEntity>(inner, theContext);
        }

        [Test]
        public void find_throws_exception_if_there_is_no_tenant()
        {
            theContext.CurrentTenant = Guid.Empty;

            Exception<InvalidOperationException>.ShouldBeThrownBy(() => {
                theStorage.Find(Guid.NewGuid());
            });
        }

        [Test]
        public void find_when_the_tenant_id_matches()
        {
            var entity = new TrackedEntity
            {
                TenantId = theContext.CurrentTenant,
                Id = Guid.NewGuid()
            };

            inner.Update(entity);

            theStorage.Find(entity.Id).ShouldBeTheSameAs(entity);
        }

        [Test]
        public void find_when_the_entity_does_not_match_the_tenant()
        {
            var entity = new TrackedEntity
            {
                TenantId = Guid.NewGuid(),
                Id = Guid.NewGuid()
            };

            inner.Update(entity);

            theStorage.Find(entity.Id).ShouldBeNull();
        }

        [Test]
        public void update_will_throw_an_exception_if_there_is_no_tenant()
        {
            theContext.CurrentTenant = Guid.Empty;

            Exception<InvalidOperationException>.ShouldBeThrownBy(() =>
            {
                theStorage.Update(new TrackedEntity());
            });
        }

        [Test]
        public void update_should_set_the_tenant_id_before_updating_to_the_inner()
        {
            var entity = new TrackedEntity
            {
                Id = Guid.NewGuid()
            };

            theStorage.Update(entity);

            entity.TenantId.ShouldBe(theContext.CurrentTenant);

            inner.Find(entity.Id).ShouldBeTheSameAs(entity);
        }

        [Test]
        public void update_will_throw_an_exception_if_you_try_to_update_an_entity_that_is_not_owned_by_the_current_tenant()
        {
            var original = new TrackedEntity
            {
                Id = Guid.NewGuid(),
                TenantId = Guid.NewGuid()
            };

            inner.Update(original);

            var newEntity = new TrackedEntity
            {
                Id = original.Id
            };

            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() => {
                theStorage.Update(newEntity);
            });
        }

        [Test]
        public void remove_throws_exception_if_no_tenant_is_detected()
        {
            theContext.CurrentTenant = Guid.Empty;

            Exception<InvalidOperationException>.ShouldBeThrownBy(() =>
            {
                theStorage.Remove(new TrackedEntity());
            });
        }

        [Test]
        public void remove_throws_exception_if_the_entity_is_owned_by_a_different_tenant()
        {
            var original = new TrackedEntity
            {
                Id = Guid.NewGuid(),
                TenantId = Guid.NewGuid()
            };

            inner.Update(original);

            var newEntity = new TrackedEntity
            {
                Id = original.Id
            };

            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                theStorage.Remove(newEntity);
            });
        }

        [Test]
        public void remove_happy_path()
        {
            var original = new TrackedEntity
            {
                Id = Guid.NewGuid(),
                TenantId = theContext.CurrentTenant
            };

            inner.Update(original);

            var newEntity = new TrackedEntity
            {
                Id = original.Id
            };

            theStorage.Remove(newEntity);

            inner.All().Any().ShouldBeFalse();
        }

        [Test]
        public void all_will_throw_exceptions_if_no_tenant_is_detected()
        {
            theContext.CurrentTenant = Guid.Empty;

            Exception<InvalidOperationException>.ShouldBeThrownBy(() => {
                theStorage.All().Each(x => Debug.WriteLine(x));
            });
        }

        [Test]
        public void all_only_returns_values_where_the_tenant_id_matches()
        {
            var e1 = new TrackedEntity {TenantId = theContext.CurrentTenant};
            var e2 = new TrackedEntity {TenantId = theContext.CurrentTenant};
            var e3 = new TrackedEntity {TenantId = Guid.NewGuid()};
            var e4 = new TrackedEntity {TenantId = theContext.CurrentTenant};
            var e5 = new TrackedEntity {TenantId = theContext.CurrentTenant};
            var e6 = new TrackedEntity { TenantId = Guid.NewGuid() };
            var e7 = new TrackedEntity {TenantId = theContext.CurrentTenant};

            inner.Update(e1);
            inner.Update(e2);
            inner.Update(e3);
            inner.Update(e4);
            inner.Update(e5);
            inner.Update(e6);
            inner.Update(e7);

            theStorage.All().ShouldHaveTheSameElementsAs(e1, e2, e4, e5, e7);
        }

        [Test]
        public void find_single_throws_exception_if_no_tenant()
        {
            theContext.CurrentTenant = Guid.Empty;

            Exception<InvalidOperationException>.ShouldBeThrownBy(() => {
                theStorage.FindSingle(x => x.Id == Guid.NewGuid());
            });
        }

        [Test]
        public void find_single_happy_path()
        {
            var original = new TrackedEntity
            {
                Id = Guid.NewGuid(),
                TenantId = theContext.CurrentTenant,
                Name = "Jeremy"
            };

            inner.Update(original);

            inner.Update(new TrackedEntity
            {
                Name = original.Name,
                TenantId = Guid.NewGuid()
            });

            theStorage.FindSingle(x => x.Name == "Jeremy").ShouldBeTheSameAs(original);
        }

        [Test]
        public void find_single_does_not_find_anything_from_the_same_tenant()
        {
            var original = new TrackedEntity
            {
                Id = Guid.NewGuid(),
                TenantId = Guid.NewGuid(),
                Name = "Jeremy"
            };

            inner.Update(original);

            theStorage.FindSingle(x => x.Name == "Jeremy")
                .ShouldBeNull();
        }
    }

    public class TrackedEntity : TenantedEntity
    {
        public string Name { get; set; }
    }
}