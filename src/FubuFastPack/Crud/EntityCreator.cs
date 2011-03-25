using FubuFastPack.Domain;
using FubuMVC.Core.Urls;
using FubuValidation;

namespace FubuFastPack.Crud
{
    public class EntityCreator<TEdit, TEntity>
        where TEdit : EditEntityModel
        where TEntity : DomainEntity
    {
        private readonly INewEntityHandler<TEdit> _newEntityHandler;
        private readonly IEntitySaver<TEntity> _saver;
        private readonly IValidator _validator;
        private readonly IUrlRegistry _urls;
        private readonly IFlattener _flattener;

        public EntityCreator(INewEntityHandler<TEdit> newEntityHandler, IEntitySaver<TEntity> saver, IValidator validator, IUrlRegistry urls, IFlattener flattener)
        {
            _newEntityHandler = newEntityHandler;
            _saver = saver;
            _validator = validator;
            _urls = urls;
            _flattener = flattener;
        }

        public CrudReport Create(CreationRequest<TEdit> request)
        {
            var input = request.Input;
            var notification = input.Notification;
            var model = (TEntity)input.Target;
            _validator.Validate(model, notification);

            var wasSaved = false;
            string editUrl = null;


            if (notification.IsValid())
            {
                _saver.Create(model);
                wasSaved = true;
                editUrl = _urls.UrlFor(model);

                _newEntityHandler.HandleNew(input);
            }

            var flattenedValue = _flattener.Flatten(model);
            return new CrudReport(notification, model, flattenedValue)
                   {
                       success = wasSaved,
                       editUrl = editUrl
                   };
        }
    }
}