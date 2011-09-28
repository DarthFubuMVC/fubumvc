using System.Collections.Generic;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Diagnostics.Features.Chains;

namespace FubuMVC.Diagnostics.Features.Requests
{
	public class RequestDetailsModel
	{
        public RequestDetailsModel()
        {
            Logs = new List<RequestLogEntry>();
        }

        public BehaviorDetailsModel Root { get; set; }
        public IEnumerable<RequestLogEntry> Logs { get; set; }
		// Leave this here for extensibility?
		public IDebugReport Report { get; set; }
        public ChainModel Chain { get; set; }
	}
}