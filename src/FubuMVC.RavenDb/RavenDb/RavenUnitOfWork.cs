using System;
using FubuMVC.RavenDb.MultiTenancy;
using StructureMap;

namespace FubuMVC.RavenDb.RavenDb
{
    public class RavenUnitOfWork : IUnitOfWork, IDisposable
    {
        private Lazy<IContainer> _nested;
        private readonly IContainer _container;

        public RavenUnitOfWork(IContainer container)
        {
            _container = container;
            reset();
        }

        private void reset()
        {
            _nested = new Lazy<IContainer>(_container.GetNestedContainer);
        }

        public IEntityRepository Start()
        {
            assertNotAlreadyStarted();

            return _nested.Value.GetInstance<IEntityRepository>();
        }

        private void assertNotAlreadyStarted()
        {
            if (_nested.IsValueCreated) throw new InvalidOperationException("Start or Start(tenantId) can only be called once");
        }

        public IEntityRepository Start(Guid tenantId)
        {
            assertNotAlreadyStarted();

            return _nested.Value.With<ITenantContext>(new SimpleTenantContext { CurrentTenant = tenantId }).GetInstance<IEntityRepository>();
        }

        public void Commit()
        {
            if (_nested.IsValueCreated)
            {
                _nested.Value.GetInstance<ISessionBoundary>().SaveChanges();
            }

            reset();
        }

        public void Reject()
        {
            Dispose();
            reset();
        }

        public void Dispose()
        {
            if (_nested.IsValueCreated)
            {
                _nested.Value.Dispose();
            }
        }
    }
}