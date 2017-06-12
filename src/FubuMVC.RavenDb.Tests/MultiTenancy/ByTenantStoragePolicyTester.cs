using System;
using FubuMVC.RavenDb.InMemory;
using FubuMVC.RavenDb.MultiTenancy;
using FubuMVC.RavenDb.Storage;
using Shouldly;
using Xunit;

namespace FubuMVC.RavenDb.Tests.MultiTenancy
{
    public class bytenantstoragepolicytester
    {
        private SimpleTenantContext theContext;
        private ByTenantStoragePolicy thePolicy;

        public bytenantstoragepolicytester()
        {
            theContext = new SimpleTenantContext
            {
                CurrentTenant = Guid.NewGuid()
            };

            thePolicy = new ByTenantStoragePolicy(theContext);
        }

        [Fact]
        public void matches_negative()
        {
            thePolicy.Matches<GlobalEntity>().ShouldBeFalse();
        }

        [Fact]
        public void matches_positive()
        {
            thePolicy.Matches<MyTenantedEntity>().ShouldBeTrue();
        }

        [Fact]
        public void wrap()
        {
            var inner = new EntityStorage<MyTenantedEntity>(new InMemoryPersistor());

            thePolicy.Wrap(inner).ShouldBeOfType<ByTenantEntityStorage<MyTenantedEntity>>()
                .Context.ShouldBeTheSameAs(theContext);
        }
    }

    public class MyTenantedEntity : TenantedEntity
    {

    }

    public class GlobalEntity : Entity
    {

    }
}
