using FubuFastPack.Domain;

namespace FubuFastPack.Crud.Properties
{
    public interface IPropertyUpdater<T> where T : DomainEntity
    {
        UpdatePropertyResultViewModel EditProperty(UpdatePropertyModel<T> updatePropertyModel);
    }
}