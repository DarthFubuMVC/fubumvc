using System;
using System.Diagnostics;
using System.Threading;

namespace FubuMVC.Core.Services
{
    public static class Wait
    {
        public static bool Until(Func<bool> condition, int millisecondPolling = 500, int timeoutInMilliseconds = 5000)
        {
            if (condition()) return true;

            var clock = new Stopwatch();
            clock.Start();

            while (clock.ElapsedMilliseconds < timeoutInMilliseconds)
            {
                Thread.Yield();
                Thread.Sleep(millisecondPolling);

                if (condition()) return true;
            }

            return false;
        }
    }
}