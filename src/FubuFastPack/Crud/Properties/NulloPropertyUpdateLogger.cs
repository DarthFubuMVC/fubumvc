using FubuFastPack.Domain;

namespace FubuFastPack.Crud.Properties
{
    public class NulloPropertyUpdateLogger<TEntity> : IPropertyUpdateLogger<TEntity> where TEntity : DomainEntity
    {
        public void Log(TEntity entity, EditPropertyResult result)
        {
            // Nothing
        }
    }
}