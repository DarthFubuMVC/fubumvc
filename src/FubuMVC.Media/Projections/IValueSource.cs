namespace FubuMVC.Media.Projections
{
    public interface IValueSource<T>
    {
        IValues<T> FindValues();
    }
}