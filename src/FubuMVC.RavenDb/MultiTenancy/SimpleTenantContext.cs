using System;
using FubuCore.Binding;

namespace FubuPersistence.MultiTenancy
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