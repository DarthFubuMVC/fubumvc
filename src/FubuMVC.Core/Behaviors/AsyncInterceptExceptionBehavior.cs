using System;
using System.Threading.Tasks;

namespace FubuMVC.Core.Behaviors
{
    public abstract class AsyncInterceptExceptionBehavior<T> : IActionBehavior where T : Exception
    {
        public IActionBehavior Inner { get; set; }

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
            var task = Task.Factory.StartNew(() => behaviorAction(Inner));
            task.ContinueWith(x =>
            {
                try
                {
                    //this will allow a catching an exception rather than inspecting task data
                    x.Wait();
                }
                catch (T exception)
                {
                    if (!ShouldHandle(exception))
                        throw;
                    Handle(exception);
                }
            }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnFaulted);
        }


        public virtual bool ShouldHandle(T exception)
		{
			return true;
		}

		public abstract void Handle(T exception);
    }
}