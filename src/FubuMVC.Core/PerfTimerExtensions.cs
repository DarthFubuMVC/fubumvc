using System;
using System.Threading.Tasks;
using Bottles;
using Bottles.Diagnostics;

namespace FubuMVC.Core
{
    public static class PerfTimerExtensions
    {
        public static Task RecordTask(this IPerfTimer timer, string description, Action action)
        {
            return Task.Factory.StartNew(() => timer.Record(description, action));
        }

        public static Task<T> RecordTask<T>(this IPerfTimer timer, string description, Func<T> func)
        {
            return Task.Factory.StartNew(() => {
                return timer.Record<T>(description, func);
            });
        }

        public static Task RecordContinuation<T>(this Task<T> task, string description, Action<Task<T>> action)
        {
            return task.ContinueWith(t => PackageRegistry.Timer.Record(description, () => action(t)));
        }
    }
}