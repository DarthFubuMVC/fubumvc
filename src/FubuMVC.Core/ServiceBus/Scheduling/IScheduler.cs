using System;

namespace FubuMVC.Core.ServiceBus.Scheduling
{
    public interface IScheduler : IDisposable
    {
        void Start(Action action);
    }
}