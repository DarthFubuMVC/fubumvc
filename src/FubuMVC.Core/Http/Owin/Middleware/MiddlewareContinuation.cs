using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FubuMVC.Core.Http.Owin.Middleware
{
    public class MiddlewareContinuation
    {
        public static MiddlewareContinuation Continue(Func<Task> action = null)
        {
            return new MiddlewareContinuation {DoNext = DoNext.Continue, Action = action};
        }

        public static MiddlewareContinuation StopHere(Func<Task> action = null)
        {
            return new MiddlewareContinuation { DoNext = DoNext.Stop, Action = action };
        }

        public DoNext DoNext;
        public Func<Task> Action;

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

        public async Task ToTask(IDictionary<string, object> environment, Func<IDictionary<string, object>, Task> inner)
        {
            if (Action != null)
            {
                await Action.Invoke().ConfigureAwait(false);
            }

            if (DoNext == DoNext.Continue)
            {
                await inner(environment).ConfigureAwait(false);
            }
        }

    }
}