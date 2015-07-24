using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuMVC.RavenDb.Storage;

namespace FubuMVC.RavenDb.MultiTenancy
{
    public class ByTenantEntityStorage<T> : IEntityStorage<T> where T : class, ITenantedEntity
    {
        private readonly IEntityStorage<T> _inner;
        private readonly ITenantContext _context;

        public ByTenantEntityStorage(IEntityStorage<T> inner, ITenantContext context)
        {
            _inner = inner;
            _context = context;

            if (_context.CurrentTenant == Guid.Empty) throw new InvalidOperationException();
        }

        public ITenantContext Context
        {
            get { return _context; }
        }

        public T Find(Guid id)
        {
            assertHasContext();

            var subject = _inner.Find(id);
            if (subject == null) return null;

            return subject.TenantId == _context.CurrentTenant ? subject : null;
        }

        public void Update(T entity)
        {
            assertHasContext();
            ifMatchesTenant(entity.Id, () =>
            {
                entity.TenantId = _context.CurrentTenant;
                _inner.Update(entity);
            });
        }

        public void Remove(T entity)
        {
            assertHasContext();
            ifMatchesTenant(entity.Id, () => _inner.Remove(entity));
        }

        public IQueryable<T> All()
        {
            assertHasContext();
            return _inner.All().Where(x => x.TenantId == _context.CurrentTenant);
        }

        // Not too worried about how awful this is because it's only used in testing with
        // very small data sets
        public void DeleteAll()
        {
            assertHasContext();

            _inner.All().Where(x => x.TenantId == _context.CurrentTenant).ToList().Each( x => _inner.Remove(x));
        }

        public T FindSingle(Expression<Func<T, bool>> filter)
        {
            assertHasContext();
            return _inner.All().Where(x => x.TenantId == _context.CurrentTenant).Where(filter).FirstOrDefault();
        }

        private void ifMatchesTenant(Guid id, Action continuation)
        {
            var subject = _inner.Find(id);
            if (subject == null || subject.TenantId == _context.CurrentTenant)
            {
                continuation();

                return;
            }

            throw new ArgumentOutOfRangeException("This entity is not owned by the current Tenant");
        }

        private void assertHasContext()
        {
            if (_context.CurrentTenant == Guid.Empty)
            {
                throw new InvalidOperationException("No current Tenant is available in this context");
            }
        }
    }
}