using System;
using System.Threading.Tasks;

namespace LightningQueues.Protocol
{
    public static class TaskExtensions
    {
        public static async Task WithTimeout(this Task task, TimeSpan timeout)
        {
            if (task == await Task.WhenAny(task, Task.Delay(timeout)).ConfigureAwait(false))
            {
                // Await the completed task to propagate exceptions that may have been thrown.
                await task;
            }
            else
            {
                throw new TimeoutException(); 
            }
        }

        public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout)
        {
            if (task == await Task.WhenAny(task, Task.Delay(timeout)).ConfigureAwait(false))
            {
                return await task;
            }

            throw new TimeoutException();
        }
    }
}