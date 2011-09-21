using FubuCore.Binding;

namespace FubuMVC.Core.Diagnostics
{
    public class DebugDetector : IDebugDetector
    {
        public static readonly string FLAG = "FubuDebug";
        private readonly IRequestData _request;

        public DebugDetector(IRequestData request)
        {
            _request = request;
        }

        public virtual bool IsDebugCall()
        {
            bool returnValue = false;
            _request.Value(FLAG, o => returnValue = true);

            return returnValue;
        }
    }
}