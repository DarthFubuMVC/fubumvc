using System;

namespace FubuPersistence.MultiTenancy
{
    public class NulloTenantContext : ITenantContext
    {
        public Guid CurrentTenant
        {
            get { return Guid.Empty; }
        }
    }
}