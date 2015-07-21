using System;

namespace FubuMVC.Core.ServiceBus.Events
{
    public interface IExpiringListener
    {
        bool IsExpired { get; }
        DateTime? ExpiresAt { get; }
    }
}