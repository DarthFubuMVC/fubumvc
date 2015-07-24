using System;

namespace FubuMVC.RavenDb.MultiTenancy
{
    public interface ITenantContext
    {
        Guid CurrentTenant { get; }
    }
}