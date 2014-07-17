using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Diagnostics.Runtime;

namespace FubuMVC.Diagnostics.Requests
{
    public class RequestsFubuDiagnostics
    {
        private readonly IRequestHistoryCache _cache;

        public RequestsFubuDiagnostics(IRequestHistoryCache cache)
        {
            _cache = cache;
        }

        public Dictionary<string, object> get_requests()
        {
            return new Dictionary<string, object>
            {
                {"requests", _cache.RecentReports().Select(x => x.ToDictionary()).ToArray()}
            };
        }
    }
}