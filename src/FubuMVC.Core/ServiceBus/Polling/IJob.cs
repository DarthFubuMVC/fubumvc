using System;
using System.Threading;
using System.Threading.Tasks;

namespace FubuMVC.Core.ServiceBus.Polling
{
    public interface IJob
    {
        void Execute(CancellationToken cancellation);
    }

    public class JobTimeout
    {
        private readonly TimeSpan _timeout;

        public JobTimeout(TimeSpan timeout)
        {
            _timeout = timeout;
        }

        public TimeSpan Timeout
        {
            get { return _timeout; }
        }

        public async Task Execute(IJob job)
        {
            var source = new CancellationTokenSource(_timeout);
            var cancellation = new CancellationToken();
            var task = Task.Factory.StartNew(() => job.Execute(source.Token), source.Token);
            var timeout = Task.Delay(_timeout, cancellation);
            if (await Task.WhenAny(task, timeout) == task)
            {
                await task;
            }
            else
            {
                var marker = new TaskCompletionSource<object>();
                marker.SetException(new TimeoutException("Timed out after " + _timeout));

                await marker.Task;
            }
        }
    }
}