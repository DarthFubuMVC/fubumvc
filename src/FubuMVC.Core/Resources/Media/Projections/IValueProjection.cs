namespace FubuMVC.Core.Resources.Media.Projections
{
    public interface IValueProjection<T>
    {
        void WriteValue(IValues<T> target, IMediaNode node);
    }
}