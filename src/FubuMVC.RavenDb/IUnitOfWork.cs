using System;

namespace FubuPersistence
{
    public interface IUnitOfWork
    {
        IEntityRepository Start();
        IEntityRepository Start(Guid tenantId);
        void Commit();
        void Reject();
    }
}