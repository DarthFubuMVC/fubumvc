namespace FubuMVC.RavenDb.Storage
{
    public interface IStorageFactory
    {
        IEntityStorage<T> StorageFor<T>() where T : class, IEntity;
    }
}