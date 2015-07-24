using System;

namespace FubuMVC.RavenDb.MultiTenancy
{
    public class NulloTenantContext : ITenantContext
    {
        public Guid CurrentTenant
        {
            get { return Guid.Empty; }
        }
    }
}