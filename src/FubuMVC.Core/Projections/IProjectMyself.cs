namespace FubuMVC.Core.Projections
{
    /// <summary>
    /// Used to make a DTO class type control its own projection for
    /// "special" one off type behavior
    /// </summary>
    public interface IProjectMyself
    {
        void Project(string attributeName, IMediaNode node);
    }
}