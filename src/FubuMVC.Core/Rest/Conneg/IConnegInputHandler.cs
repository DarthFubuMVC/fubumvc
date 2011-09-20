using FubuCore;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Rest.Conneg
{
    [MarkedForTermination]
    public interface IConnegInputHandler
    {
        void ReadInput(CurrentRequest currentRequest, IFubuRequest request);
    }
}