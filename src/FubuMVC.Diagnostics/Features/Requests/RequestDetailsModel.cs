using FubuMVC.Core.Diagnostics;

namespace FubuMVC.Diagnostics.Features.Requests
{
	public class RequestDetailsModel
	{
		// Leave this here for extensibility
		public IDebugReport Report { get; set; }

		public BehaviorDetailsModel Root { get; set; }
	}
}