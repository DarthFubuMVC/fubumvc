using FubuCore;

namespace FubuMVC.Core.Diagnostics
{
    public class RequestObserver : IRequestObserver
    {
        private readonly IDebugReport _report;

        public RequestObserver(IDebugReport report)
        {
            _report = report;
        }

        public void RecordLog(string message)
        {
            _report
                .AddDetails(new RequestLogEntry
                                {
                                    Message = message
                                });
        }

        public void RecordLog(string message, params object[] args)
        {
            RecordLog(message.ToFormat(args));
        }
    }
}