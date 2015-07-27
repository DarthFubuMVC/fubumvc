using System;
using FubuMVC.RavenDb.InMemory;
using FubuMVC.RavenDb.MultiTenancy;
using FubuMVC.RavenDb.Storage;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.RavenDb.Tests.MultiTenancy
{
    [TestFixture]
    public class ByTenantStoragePolicyTester
    {
        private SimpleTenantContext theContext;
        private ByTenantStoragePolicy thePolicy;

        [SetUp]
        public void SetUp()
        {
            theContext = new SimpleTenantContext
            {
                CurrentTenant = Guid.NewGuid()
            };

            thePolicy = new ByTenantStoragePolicy(theContext);
        }

        [Test]
        public void matches_negative()
        {
            thePolicy.Matches<GlobalEntity>().ShouldBeFalse();
        }

        [Test]
        public void matches_positive()
        {
            thePolicy.Matches<MyTenantedEntity>().ShouldBeTrue();
        }

        [Test]
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