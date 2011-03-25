using FubuFastPack.Domain;
using FubuFastPack.Persistence;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;

namespace FubuFastPack.Crud
{
    public class EnsureEntityExistsBehavior<TEntity> : BasicBehavior where TEntity : DomainEntity
    {
        private readonly IFubuRequest _request;
        private readonly IRepository _repository;

        public EnsureEntityExistsBehavior(IFubuRequest request, IRepository repository)
            : base(PartialBehavior.Executes)
        {
            _request = request;
            _repository = repository;
        }

        protected override DoNext performInvoke()
        {
            var item = _request.Get<ItemRequest>();
            var entity = _repository.FindRequired<TEntity>(item.Id);
            _request.Set(entity);
            return DoNext.Continue;
        }
    }
}