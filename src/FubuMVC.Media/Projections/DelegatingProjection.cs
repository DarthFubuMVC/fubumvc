namespace FubuMVC.Media.Projections
{
    public class DelegatingProjection<T, TProjection> : IProjection<T> where TProjection : IProjection<T>, new()
    {
        public void Write(IProjectionContext<T> context, IMediaNode node)
        {
            new TProjection().Write(context, node);
        }
    }
}