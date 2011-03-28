using System.Collections.Generic;

namespace FubuMVC.Diagnostics.Models.Requests
{
    public class RequestCacheModel
    {
        public RequestCacheModel()
        {
            BehaviorReports = new List<RecordedRequestModel>();
        }

        public IEnumerable<RecordedRequestModel> BehaviorReports { get; set; }
    }
}