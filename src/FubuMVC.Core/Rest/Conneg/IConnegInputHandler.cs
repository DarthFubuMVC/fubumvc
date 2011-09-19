using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Rest.Conneg
{
    public interface IConnegInputHandler
    {
        void ReadInput(CurrentRequest currentRequest, IFubuRequest request);
    }
}