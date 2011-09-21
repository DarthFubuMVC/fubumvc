namespace FubuMVC.Core.Rest.Media
{
    public interface IValueSource<T>
    {
        IValues<T> FindValues();
    }
}