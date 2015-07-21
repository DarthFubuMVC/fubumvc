using System;
using System.Threading;
using System.Threading.Tasks;

namespace FubuMVC.Core.ServiceBus
{
    public static class TaskExtensions
    {
        public static Task<T> ToCompletionTask<T>(this T value)
        {
            var completion = new TaskCompletionSource<T>();
            completion.SetResult(value);

            return completion.Task;
        }

        public static Task ToFaultedTask(this Exception ex)
        {
            var completion = new TaskCompletionSource<object>();
            completion.SetException(ex);

            return completion.Task;
        }

        public static async Task TimeoutAfter(this Task task, TimeSpan timeout)
        {
            var cancellation = new CancellationTokenSource();
            var delayed = Task.Delay(timeout, cancellation.Token);

            if (task == await Task.WhenAny(task, delayed))
            {
                cancellation.Cancel();
                await task;
            }
            else
            {
                throw new TimeoutException();
            }
        }
    }
}