using System.Collections.Generic;
using FubuMVC.Core.Diagnostics;

namespace FubuMVC.Diagnostics.Features.Requests
{
	public class RequestDetailsModel
	{
        public RequestDetailsModel()
        {
            Logs = new List<RequestLogEntry>();
        }

		// Leave this here for extensibility
		public IDebugReport Report { get; set; }
		public BehaviorDetailsModel Root { get; set; }
        public IEnumerable<RequestLogEntry> Logs { get; set; }
	}
}