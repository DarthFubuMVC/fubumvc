namespace FubuMVC.RavenDb.Storage
{
    public interface IEntityStoragePolicy
    {
        bool Matches<T>() where T : class, IEntity;
        IEntityStorage<T> Wrap<T>(IEntityStorage<T> inner) where T : class, IEntity;
    }
}