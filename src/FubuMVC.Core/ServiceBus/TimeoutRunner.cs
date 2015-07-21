using System;
using System.Threading;
using FubuCore;

namespace FubuMVC.Core.ServiceBus
{
    public enum Completion
    {
        Success,
        Timedout,
        Exception
    }

    public static class TimeoutRunner
    {
        public static Completion Run(TimeSpan timeout, Action action, Action<Exception> onError)
        {
            var returnValue = Completion.Success;

            var reset = new ManualResetEvent(false);
            var started = new ManualResetEvent(false);

            var thread = new Thread(() => {
                try
                {
                    started.Set();
                    action();
                    reset.Set();
                    returnValue = Completion.Success;
                }
                catch (ThreadAbortException ex)
                {
                    returnValue = Completion.Timedout;
                    onError(ex);
                }
                catch (Exception ex)
                {
                    returnValue = Completion.Exception;
                    onError(ex);
                    reset.Set();
                }
            });

            thread.Start();
            started.WaitOne(1.Minutes()); // This is for making tests more reliable

            if (!reset.WaitOne(timeout))
            {
                thread.Abort();
                returnValue = Completion.Timedout;
            }

            return returnValue;
        }

        
    }
}