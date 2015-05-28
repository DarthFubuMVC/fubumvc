namespace FubuPersistence.Storage
{
    public interface IStorageFactory
    {
        IEntityStorage<T> StorageFor<T>() where T : class, IEntity;
    }
}