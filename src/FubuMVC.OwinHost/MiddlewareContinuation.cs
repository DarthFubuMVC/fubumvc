using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuMVC.Core;

namespace FubuMVC.OwinHost
{
    public class MiddlewareContinuation
    {
        public static MiddlewareContinuation Continue(Action action = null)
        {
            return new MiddlewareContinuation {DoNext = DoNext.Continue, Action = action};
        }

        public static MiddlewareContinuation StopHere(Action action = null)
        {
            return new MiddlewareContinuation { DoNext = DoNext.Stop, Action = action };
        }

        public DoNext DoNext;
        public Action Action;

        public Task ToTask(IDictionary<string, object> environment, Func<IDictionary<string, object>, Task> inner)
        {
            if (DoNext == DoNext.Continue)
            {
                if (Action != null)
                {
                    Action();
                }

                return inner(environment);
            }

            return Task.Factory.StartNew(Action ?? (() => { }));
        }
    }
}