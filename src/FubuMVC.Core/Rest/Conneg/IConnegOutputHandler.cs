using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Rest.Conneg
{
    public interface IConnegOutputHandler
    {
        void WriteOutput(CurrentRequest currentRequest, IFubuRequest request);
    }
}