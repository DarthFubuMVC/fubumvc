using FubuCore.Binding;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Runtime;
using FubuMVC.Diagnostics.Features.Requests;

namespace FubuMVC.Diagnostics.Core.Infrastructure
{
    public class AdvancedDebugDetector : DebugDetector
    {
        private readonly IFubuRequest _request;

        public AdvancedDebugDetector(IRequestData requestData, IFubuRequest request)
            : base(requestData)
        {
            _request = request;
        }

        public override bool IsDebugCall()
        {
            return base.IsDebugCall() && !_request.Has<RecordedRequestRequestModel>();
        }
    }
}