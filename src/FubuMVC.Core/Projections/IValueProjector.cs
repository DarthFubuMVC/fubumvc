namespace FubuMVC.Core.Projections
{
    public interface IValueProjector<T>
    {
        void Project(string attributeName, T value, IMediaNode node);
    }
}