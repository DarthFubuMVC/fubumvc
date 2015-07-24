using System;
using FubuCore.Binding;

namespace FubuMVC.RavenDb.MultiTenancy
{
    public class SimpleTenantContext : ITenantContext
    {
        public Guid CurrentTenant { get; set; }

        public static ServiceArguments ArgumentsForTenant(Guid tenantId)
        {
            return new ServiceArguments().With<ITenantContext>(new SimpleTenantContext{CurrentTenant = tenantId});
        }
    }
}