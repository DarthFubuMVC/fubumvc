using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FubuMVC.Core.Behaviors
{
    public abstract class AsyncInterceptExceptionBehavior<T> : IActionBehavior where T : Exception
    {
        public IActionBehavior InsideBehavior { get; set; }

        public void Invoke()
        {
            InnerInvoke(x => x.Invoke());
        }

        public void InvokePartial()
        {
            InnerInvoke(x => x.InvokePartial());
        }

        private void InnerInvoke(Action<IActionBehavior> behaviorAction)
        {
			if (InsideBehavior == null)
				throw new FubuAssertionException("When intercepting exceptions you must have an inside behavior. Otherwise, there would be nothing to intercept.");

            var task = Task.Factory.StartNew(() => behaviorAction(InsideBehavior));
            task.ContinueWith(x =>
            {
                try
                {
                    //this will allow a catching an exception rather than inspecting task data
                    x.Wait();
                }
                catch (AggregateException e)
                {
                    //Multiple exceptions can occur, this will allow behavior chain to handle
                    //in the same way
                    var aggregateException = e.Flatten();
                    var remaining = TryHandle(aggregateException.InnerExceptions);
                    if(remaining.Any())
                        throw new AggregateException(remaining);
                }
            }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnFaulted);
        }

        public IEnumerable<Exception> TryHandle(IEnumerable<Exception> exceptions)
        {
            foreach (var exception in exceptions.OfType<T>())
            {
                if (ShouldHandle(exception))
                    Handle(exception);
                else
                    yield return exception;
            }
        }

        public virtual bool ShouldHandle(T exception)
		{
			return true;
		}

		public abstract void Handle(T exception);
    }
}