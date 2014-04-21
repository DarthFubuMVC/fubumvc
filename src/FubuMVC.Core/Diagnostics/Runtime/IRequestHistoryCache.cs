using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Diagnostics.Runtime
{
    public interface IRequestHistoryCache
    {
        void Store(RequestLog log);
        IEnumerable<RequestLog> RecentReports();

        RequestLog Find(Guid id);
    }
}