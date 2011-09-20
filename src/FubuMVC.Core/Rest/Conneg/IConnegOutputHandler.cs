using FubuCore;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Rest.Conneg
{
    [MarkedForTermination]
    public interface IConnegOutputHandler
    {
        void WriteOutput(CurrentRequest currentRequest, IFubuRequest request);
    }
}