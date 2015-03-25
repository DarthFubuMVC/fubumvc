using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using OpenQA.Selenium.Support.UI;

namespace Serenity
{
    public static class Wait
    {
        public static bool Until(Func<bool> condition, int millisecondPolling = 500, int timeoutInMilliseconds = 5000)
        {
            if (condition()) return true;

            var clock = new Stopwatch();
            clock.Start();

            return Until(condition, clock, millisecondPolling, timeoutInMilliseconds);
        }

        private static readonly object _NO_OP_ = new object();

        public static T For<T>(Func<T> condition, TimeSpan? pollingInterval = null, TimeSpan? timeout = null, Type[] ignoreExceptions = null)
        {
            var wait = new DefaultWait<object>(_NO_OP_);

            if (ignoreExceptions != null)
            {
                wait.IgnoreExceptionTypes(ignoreExceptions.ToArray());
            }

            wait.PollingInterval = pollingInterval ?? TimeSpan.FromMilliseconds(500);
            wait.Timeout = timeout ?? TimeSpan.FromSeconds(5);

            return wait.Until(x => condition());
        }

        public static bool Until(IEnumerable<Func<bool>> conditions, int millisecondPolling = 500, int timeoutInMilliseconds = 5000)
        {
            if (conditions == null)
                throw new ArgumentNullException("conditions");

            var clock = new Stopwatch();
            clock.Start();

            return conditions
                .Where(condition => !condition())
                .All(condition => Until(condition, clock, millisecondPolling, timeoutInMilliseconds));
        }

        private static bool Until(Func<bool> condition, Stopwatch clock, int millisecondPolling, int timeoutInMilliseconds)
        {
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