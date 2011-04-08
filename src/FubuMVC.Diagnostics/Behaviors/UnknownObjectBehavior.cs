using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Diagnostics.Models;

namespace FubuMVC.Diagnostics.Behaviors
{
	public class UnknownObjectBehavior : IActionBehavior
	{
		private readonly IFubuRequest _request;
		private readonly IPartialFactory _factory;
		private readonly IActionBehavior _inner;

		public UnknownObjectBehavior(IFubuRequest request, IPartialFactory factory, IActionBehavior inner)
		{
			_request = request;
			_inner = inner;
			_factory = factory;
		}

		public void Invoke()
		{
			if(_inner == null)
			{
				return;
			}

			try
			{
				_inner.Invoke();
			}
			catch (UnknownObjectException exc)
			{
				var unknownRequest = new UnknownChainRequest {Id = exc.Id};
				_request.Set(unknownRequest);
				_factory
					.BuildPartial(unknownRequest.GetType())
					.Invoke();
			}
		}

		public void InvokePartial()
		{
			Invoke();
		}
	}
}