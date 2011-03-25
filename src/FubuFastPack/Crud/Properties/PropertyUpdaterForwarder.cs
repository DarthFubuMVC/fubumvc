using FubuFastPack.Domain;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Urls;

namespace FubuFastPack.Crud.Properties
{
    public class PropertyUpdaterForwarder<T> : ChainForwarder<T> where T : DomainEntity
    {
        public PropertyUpdaterForwarder()
            : base(entity => new UpdatePropertyModel<T>(), Categories.PROPERTY_EDIT)
        {
        }
    }
}