using FubuFastPack.Domain;
using FubuFastPack.Persistence;

namespace FubuFastPack.Crud
{
    public class EntitySaver<T> : IEntitySaver<T> where T : DomainEntity
    {
        private readonly IRepository _repository;

        public EntitySaver(IRepository repository)
        {
            _repository = repository;
        }

        public void Create(T target)
        {
            _repository.Save(target);
        }
    }
}