using System;

namespace FubuPersistence.MultiTenancy
{
    public interface ITenantContext
    {
        Guid CurrentTenant { get; }
    }
}