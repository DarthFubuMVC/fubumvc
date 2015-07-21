using System;

namespace FubuMVC.Core.ServiceBus.Polling
{
    public interface ITimer : IDisposable
    {
        void Start(Action callback, double interval);
        void Restart();
        void Stop();

        bool Enabled { get; }
        double Interval { get; set; }
    }
}