namespace FubuMVC.Core.Diagnostics.Runtime
{
    public class RequestHistoryObserver: IRequestTraceObserver
    {
        private readonly IRequestHistoryCache _cache;

        public RequestHistoryObserver(IRequestHistoryCache cache)
        {
            _cache = cache;
        }

        public void Started(RequestLog log)
        {
            _cache.Store(log);
        }

        public void Completed(RequestLog log)
        {
            //Nothing to do here.
        }
    }
}