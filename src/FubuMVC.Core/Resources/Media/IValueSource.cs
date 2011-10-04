namespace FubuMVC.Core.Resources.Media
{
    public interface IValueSource<T>
    {
        IValues<T> FindValues();
    }
}