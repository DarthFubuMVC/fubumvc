namespace FubuMVC.Core.Rest.Conneg
{
    public interface IFormatter
    {
        T Read<T>(CurrentRequest currentRequest);
        void Write<T>(T target, CurrentRequest request);
        bool Matches(CurrentRequest request);
    }
}