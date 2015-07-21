using System;

namespace FubuMVC.Core.ServiceBus.Events
{
    public static class ExpiringListenerExtensions
    {
        public static bool IsExpired(this object o, DateTime currentTime)
        {
            var expiring = o as IExpiringListener;

            if (expiring == null) return false;

            return expiring.IsExpired || (expiring.ExpiresAt.HasValue && expiring.ExpiresAt.Value <= currentTime);
        }
    }
}