using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Runtime;
using FubuMVC.Diagnostics.Features.Requests;

namespace FubuMVC.Diagnostics.Core.Infrastructure
{
    public class DiagnosticsDebugCallHandler : IDebugCallHandler
    {
        private readonly IDebugReport _report;
        private readonly IFubuRequest _request;
        private readonly IPartialFactory _partialFactory;

        public DiagnosticsDebugCallHandler(IDebugReport report, IFubuRequest request, IPartialFactory partialFactory)
        {
            _report = report;
            _partialFactory = partialFactory;
            _request = request;
        }

        public void Handle()
        {
            var requestModel = new RecordedRequestRequestModel {Id = _report.Id};
            _request.Set(requestModel);
            _partialFactory
                .BuildPartial(requestModel.GetType())
                .Invoke();
        }
    }
}