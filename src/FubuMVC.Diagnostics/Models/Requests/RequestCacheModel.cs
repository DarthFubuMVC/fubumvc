using System.Collections.Generic;

namespace FubuMVC.Diagnostics.Models.Requests
{
    public class RequestCacheModel
    {
        public RequestCacheModel()
        {
            Requests = new List<RecordedRequestModel>();
        }

        public IEnumerable<RecordedRequestModel> Requests { get; set; }
    }
}