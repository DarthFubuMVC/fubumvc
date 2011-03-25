using FubuFastPack.Domain;

namespace FubuFastPack.Crud.Properties
{
    public interface IPropertyHandler<T> where T : DomainEntity
    {
        EditPropertyResult EditProperty(UpdatePropertyModel<T> update, T entity);
        bool CanEdit(UpdatePropertyModel<T> update);

        PropertyToUpdate FindProperty(UpdatePropertyModel<T> update);
    }
}