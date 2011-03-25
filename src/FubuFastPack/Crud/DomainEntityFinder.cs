using FubuFastPack.Domain;
using FubuFastPack.Persistence;

namespace FubuFastPack.Crud
{
    public class DomainEntityFinder<T> where T : DomainEntity
    {
        private readonly IFlattener _flattener;
        private readonly IRepository _repository;

        public DomainEntityFinder(IFlattener flattener, IRepository repository)
        {
            _flattener = flattener;
            _repository = repository;
        }

        public EntityFindViewModel Find(FindItemRequest<T> input)
        {
            var model = _repository.Find<T>(input.Id);
            var dto = _flattener.Flatten(model);

            return new EntityFindViewModel { Model = dto };
        }
    }
}