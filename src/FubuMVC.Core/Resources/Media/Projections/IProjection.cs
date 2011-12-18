namespace FubuMVC.Core.Resources.Media.Projections
{
    public interface IProjection<T>
    {
        void Write(IProjectionContext<T> context, IMediaNode node);
    }
}