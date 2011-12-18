namespace FubuMVC.Core.Resources.Media.Projections
{
    public interface IValueProjection<T>
    {
        void WriteValue(IProjectionContext<T> target, IMediaNode node);
    }
}