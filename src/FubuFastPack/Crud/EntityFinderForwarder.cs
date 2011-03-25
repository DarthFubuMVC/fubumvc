using FubuFastPack.Domain;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Urls;

namespace FubuFastPack.Crud
{
    public class EntityFinderForwarder<T> : ChainForwarder<T> where T : DomainEntity
    {
        public EntityFinderForwarder()
            : base(entity => new FindItemRequest<T>() { Id = entity.Id }, Categories.FIND)
        {
        }
    }
}