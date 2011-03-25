using System;
using System.Collections.Generic;
using FubuMVC.Core.Diagnostics;

namespace FubuMVC.Diagnostics.Models.Requests
{
    public class BehaviorReportModel
    {
        public BehaviorReportModel()
        {
            FormData = new Dictionary<string, object>();
            Details = new List<IBehaviorDetails>();
        }

        public string Url { get; set; }
        public DateTime Time { get; set; }
        public IDictionary<string, object> FormData { get; set; }
        public Type BehaviorType { get; set; }
        public string Description { get; set; }
        public IEnumerable<IBehaviorDetails> Details { get; set; }
    }
}