using System.Collections.Generic;

namespace FubuMVC.Diagnostics.Features.Requests
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