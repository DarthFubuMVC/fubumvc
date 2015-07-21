using System;
using System.Threading.Tasks;
using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.Core.Registration
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
    }
}