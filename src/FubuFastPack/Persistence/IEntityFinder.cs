namespace FubuFastPack.Persistence
{
    public interface IEntityFinder
    {
        T Find<T>(IRepository repository, string text);
    }
}