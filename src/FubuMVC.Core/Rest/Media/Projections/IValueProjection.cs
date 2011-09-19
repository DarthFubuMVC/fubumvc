namespace FubuMVC.Core.Rest.Media.Projections
{
    public interface IValueProjection<T>
    {
        void WriteValue(IValues<T> target, IMediaNode node);
    }
}