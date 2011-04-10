namespace FubuMVC.Core.Conneg
{
    public interface IFormatter
    {
        T Read<T>();
        void Write<T>(T target);
        bool Matches(CurrentRequest request);
    }
}