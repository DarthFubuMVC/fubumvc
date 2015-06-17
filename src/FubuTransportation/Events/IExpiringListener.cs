using System;

namespace FubuTransportation.Events
{
    public interface IExpiringListener
    {
        bool IsExpired { get; }
        DateTime? ExpiresAt { get; }
    }
}