using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuMVC.Core;

namespace FubuMVC.OwinHost.Middleware
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

        public void AssertOnlyContinuesToTheInner()
        {
            if (Action != null)
            {
                throw new Exception("There is an action on this continuation");
            }

            if (DoNext == DoNext.Stop)
            {
                throw new Exception("The continuation is 'DoNext.Stop'");
            }
        }

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