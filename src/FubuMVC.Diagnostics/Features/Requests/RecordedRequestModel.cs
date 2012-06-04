using System;
using System.Collections.Generic;
using FubuMVC.Diagnostics.Runtime;

namespace FubuMVC.Diagnostics.Features.Requests
{
    public class RecordedRequestModel
    {
        public RecordedRequestModel()
        {
            FormData = new Dictionary<string, object>();
            Steps = new List<BehaviorStep>();
        }

		public Guid Id { get; set; }
        public string Url { get; set; }
		public double ExecutionTime { get; set; }
        public DateTime Time { get; set; }
        public IDictionary<string, object> FormData { get; set; }
		public IEnumerable<BehaviorStep> Steps { get; set; }

		public bool HasErrors()
		{
			return GetVisitor().HasExceptions();
		}

		public string Status()
		{
			var visitor = new RecordedRequestBehaviorVisitor();
			Steps.Each(s => s.Details.AcceptVisitor(visitor));

			return visitor.StatusCode.ToString();
		}

		public string Exceptions()
		{
			return GetVisitor().Exceptions();
		}

		public RecordedRequestBehaviorVisitor GetVisitor()
		{
			var visitor = new RecordedRequestBehaviorVisitor();
			Steps.Each(s => s.Details.AcceptVisitor(visitor));
			return visitor;
		}
    }
}