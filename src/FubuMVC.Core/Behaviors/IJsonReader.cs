namespace FubuMVC.Core.Behaviors
{
    public interface IJsonReader
    {
        T Read<T>();
    }
}