using System;
using FubuPersistence.MultiTenancy;
using StructureMap;

namespace FubuPersistence.InMemory
{
    public class InMemoryUnitOfWork : IUnitOfWork
    {
        private readonly IContainer _container;

        public InMemoryUnitOfWork(IContainer container)
        {
            _container = container;
        }

        public IEntityRepository Start()
        {
            return _container.GetInstance<IEntityRepository>();
        }

        public IEntityRepository Start(Guid tenantId)
        {
            return _container
                .With<ITenantContext>(new SimpleTenantContext {CurrentTenant = tenantId})
                .GetInstance<IEntityRepository>();
        }

        public void Commit()
        {
            // no-op;
        }

        public void Reject()
        {
            // no-op;
        }
    }


}