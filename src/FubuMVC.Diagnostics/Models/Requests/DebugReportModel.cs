using System.Collections.Generic;

namespace FubuMVC.Diagnostics.Models.Requests
{
    public class DebugReportModel
    {
        public DebugReportModel()
        {
            BehaviorReports = new List<BehaviorReportModel>();
        }

        public IEnumerable<BehaviorReportModel> BehaviorReports { get; set; }
    }
}