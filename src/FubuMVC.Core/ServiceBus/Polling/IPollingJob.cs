using System;
using System.Threading.Tasks;

namespace FubuMVC.Core.ServiceBus.Polling
{
    public interface IPollingJob : IDisposable
    {
        bool IsRunning();
        void Start();
        void RunNow();
        void Stop();

        Type JobType { get; }
        ScheduledExecution ScheduledExecution { get; }
        PollingJobChain Chain { get; }
        double Interval { get; }
        Task WaitForJobToExecute();
    }
}