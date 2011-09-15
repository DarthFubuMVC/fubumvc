namespace FubuMVC.Core.Projections
{
    public interface IValueProjection
    {
        void WriteValue(IProjectionTarget target, IMediaNode node);
    }
}