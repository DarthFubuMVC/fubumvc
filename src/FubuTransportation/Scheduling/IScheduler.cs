using System;

namespace FubuTransportation.Scheduling
{
    public interface IScheduler : IDisposable
    {
        void Start(Action action);
    }
}