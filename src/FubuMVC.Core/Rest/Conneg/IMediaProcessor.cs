using FubuCore;

namespace FubuMVC.Core.Rest.Conneg
{
    [MarkedForTermination]
    public interface IMediaProcessor<T>
    {
        T Retrieve(CurrentRequest request);
        void Write(T target, CurrentRequest request);
    }
}