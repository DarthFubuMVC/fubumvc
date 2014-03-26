using FubuCore.Descriptions;
using FubuMVC.Core.Urls;
using FubuMVC.Diagnostics.Runtime;
using HtmlTags;

namespace FubuMVC.Diagnostics.Requests
{
    public class RequestsFubuDiagnostics
    {
        private readonly IRequestHistoryCache _cache;
        private readonly IUrlRegistry _urls;

        public RequestsFubuDiagnostics(IRequestHistoryCache cache, IUrlRegistry urls)
        {
            _cache = cache;
            _urls = urls;
        }

        [System.ComponentModel.Description("Requests:Request visualization for the most recent requests")]
        public RequestsViewModel get_requests()
        {
            return new RequestsViewModel
            {
                Table = new RequestTable(_urls, _cache.RecentReports())
            };
        }
    }

    public class RequestsViewModel{
        public RequestTable Table { get; set; }
    }
}