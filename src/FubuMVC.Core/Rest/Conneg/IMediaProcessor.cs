namespace FubuMVC.Core.Rest.Conneg
{
    public interface IMediaProcessor<T>
    {
        T Retrieve(CurrentRequest request);
        void Write(T target, CurrentRequest request);
    }
}