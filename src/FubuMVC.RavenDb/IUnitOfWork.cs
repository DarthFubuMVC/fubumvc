using System;

namespace FubuMVC.RavenDb
{
    public interface IUnitOfWork
    {
        IEntityRepository Start();
        IEntityRepository Start(Guid tenantId);
        void Commit();
        void Reject();
    }
}