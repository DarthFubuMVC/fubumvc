namespace FubuMVC.Core.Resources.Media.Projections
{
    public class DelegatingProjection<T, TProjection> : IValueProjection<T> where TProjection : IValueProjection<T>
    {
        public void WriteValue(IProjectionContext<T> target, IMediaNode node)
        {
            target.Service<TProjection>().WriteValue(target, node);
        }
    }
}