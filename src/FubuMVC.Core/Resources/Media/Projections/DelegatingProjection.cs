namespace FubuMVC.Core.Resources.Media.Projections
{
    public class DelegatingProjection<T, TProjection> : IProjection<T> where TProjection : IProjection<T>
    {
        public void Write(IProjectionContext<T> context, IMediaNode node)
        {
            context.Service<TProjection>().Write(context, node);
        }
    }
}