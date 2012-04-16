namespace FubuMVC.Core.Projections
{
    public interface IValueSource<T>
    {
        IValues<T> FindValues();
    }
}