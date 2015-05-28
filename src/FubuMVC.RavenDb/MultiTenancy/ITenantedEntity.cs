using System;

namespace FubuPersistence.MultiTenancy
{
    public interface ITenantedEntity : IEntity
    {
        Guid TenantId { get; set; }
    }

    public abstract class TenantedEntity : Entity, ITenantedEntity
    {
        public Guid TenantId { get; set; }
    }
}