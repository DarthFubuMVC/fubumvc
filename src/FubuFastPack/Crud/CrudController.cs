using FubuFastPack.Domain;

namespace FubuFastPack.Crud
{
    public interface CrudController<TEntity, TEdit>
        where TEntity : DomainEntity, new()
        where TEdit : EditEntityModel
    {
        TEdit Edit(TEntity model);
        CreationRequest<TEdit> Create(TEdit input);
        TEntity New();
    }
}