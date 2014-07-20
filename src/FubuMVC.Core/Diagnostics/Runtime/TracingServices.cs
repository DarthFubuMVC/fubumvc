using FubuCore.Binding.InMemory;
using FubuMVC.Core.Diagnostics.Runtime.Tracing;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Diagnostics.Runtime
{
    public class TracingServices : ServiceRegistry
    {
        public TracingServices()
        {
            // does no harm
            ReplaceService<IBindingHistory, BindingHistory>();
            SetServiceIfNone<IRequestHistoryCache, RequestHistoryCache>();

            AddService<IRequestTraceObserver, RequestHistoryObserver>();


            SetServiceIfNone<IRequestTrace, RequestTrace>();
            SetServiceIfNone<IRequestLogBuilder, RequestLogBuilder>();
        }
    }
}