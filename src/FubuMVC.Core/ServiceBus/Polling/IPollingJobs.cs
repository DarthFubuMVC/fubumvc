using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FubuMVC.Core.ServiceBus.Polling
{
    public interface IPollingJobs : IEnumerable<IPollingJob>
    {
        bool IsActive<T>() where T : IJob;
        bool IsActive(Type jobType);
        void Activate<T>() where T : IJob;
        void Activate(Type type);
        Task WaitForJobToExecute<T>() where T : IJob;
        Task ExecuteJob<T>() where T : IJob;
    }
}