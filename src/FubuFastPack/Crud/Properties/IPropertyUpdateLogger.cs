using FubuFastPack.Domain;

namespace FubuFastPack.Crud.Properties
{
    public interface IPropertyUpdateLogger<TEntity> where TEntity : DomainEntity
    {
        void Log(TEntity entity, EditPropertyResult result);
    }
}