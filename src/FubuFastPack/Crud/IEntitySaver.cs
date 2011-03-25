using FubuFastPack.Domain;

namespace FubuFastPack.Crud
{
    public interface IEntitySaver<T> where T : DomainEntity
    {
        void Create(T target);
    }
}