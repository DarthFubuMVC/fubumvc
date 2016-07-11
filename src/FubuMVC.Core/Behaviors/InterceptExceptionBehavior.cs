using System;
using System.Threading.Tasks;
using FubuCore;

namespace FubuMVC.Core.Behaviors
{
	public abstract class InterceptExceptionBehavior<T> : IActionBehavior
		where T : Exception
	{
		public IActionBehavior InsideBehavior { get; set; }

		public async Task InvokePartial()
		{
		    if (InsideBehavior != null)
		    {
		        await InsideBehavior.InvokePartial().ConfigureAwait(false);
		    }
		}

		public async Task Invoke()
		{
			if (InsideBehavior == null)
				throw new FubuAssertionException("When interception exceptions you must have an inside behavior. Otherwise, there would be nothing to intercept.");

			try
			{
				await InsideBehavior.Invoke().ConfigureAwait(false);
			}
			catch (T exception)
			{
				if (!ShouldHandle(exception))
					throw;

				Handle(exception);
			}
		}

		public virtual bool ShouldHandle(T exception)
		{
			return true;
		}

		public abstract void Handle(T exception);
	}
}