using System;

namespace FubuMVC.Core.Behaviors
{
	public abstract class InterceptExceptionBehavior<T> : IActionBehavior
		where T : Exception
	{
		public IActionBehavior InsideBehavior { get; set; }

		public void InvokePartial()
		{
			if (InsideBehavior != null)
				InsideBehavior.InvokePartial();
		}

		public void Invoke()
		{
			if (InsideBehavior == null)
				throw new FubuAssertionException("When interception exceptions you must have an inside behavior. Otherwise, there would be nothing to intercept.");

			try
			{
				InsideBehavior.Invoke();
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