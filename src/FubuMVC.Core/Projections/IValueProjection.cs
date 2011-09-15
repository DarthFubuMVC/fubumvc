namespace FubuMVC.Core.Projections
{
    public interface IValueProjection<T>
    {
        void WriteValue(IValueSource<T> target, IMediaNode node);
    }
}