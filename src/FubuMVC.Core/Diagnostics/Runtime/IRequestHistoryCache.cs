using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Diagnostics.Runtime
{
    public interface IRequestHistoryCache
    {
        void Store(RequestLog log);
        IEnumerable<RequestLog> RecentReports();

        string CurrentSessionTag { get; set; }

        RequestLog Find(Guid id);
        void Clear();
    }
}