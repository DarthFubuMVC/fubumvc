using FubuFastPack.Domain;

namespace FubuFastPack.Crud.Properties
{
    public interface ISimplePropertyHandler<T> : IPropertyHandler<T> where T : DomainEntity
    {
    }
}