using System;
using System.Threading.Tasks;

namespace FubuTransportation.Polling
{
    public interface IPollingJob : IDisposable
    {
        bool IsRunning();
        void Start();
        void RunNow();
        void Stop();

        Type JobType { get; }
        ScheduledExecution ScheduledExecution { get; }
        Task WaitForJobToExecute();
    }
}