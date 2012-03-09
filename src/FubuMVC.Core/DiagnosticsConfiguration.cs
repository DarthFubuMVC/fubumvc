using System;
using System.Collections.Generic;
using FubuMVC.Core.Diagnostics;

namespace FubuMVC.Core
{
    public class DiagnosticsConfiguration
    {
        public int MaxRequests { get; set; }
    }

    public interface IDiagnosticsConfigurationExpression
    {
        void LimitRecordingTo(int nrRequests);
        void ExcludeRequests(IRequestHistoryCacheFilter filter);
    }

    public static class DiagnosticsConfigurationExtensions
    {
        public static void ApplyFilter<TFilter>(this IDiagnosticsConfigurationExpression config)
            where TFilter : IRequestHistoryCacheFilter, new()
        {
            config.ExcludeRequests(new TFilter());
        }

        public static void ExcludeRequests(this IDiagnosticsConfigurationExpression config,
                                           Func<CurrentRequest, bool> shouldExclude)
        {
            config.ExcludeRequests(new LambdaRequestHistoryCacheFilter(shouldExclude));
        }
    }

    public class DiagnosticsConfigurationExpression : IDiagnosticsConfigurationExpression
    {
        private readonly IList<IRequestHistoryCacheFilter> _filters;

        public DiagnosticsConfigurationExpression(IList<IRequestHistoryCacheFilter> filters)
        {
            _filters = filters;
        }

        public int MaxRequests { get; private set; }

        public void LimitRecordingTo(int nrRequests)
        {
            MaxRequests = nrRequests;
        }

        public void ExcludeRequests(IRequestHistoryCacheFilter filter)
        {
            _filters.Fill(filter);
        }
    }
}