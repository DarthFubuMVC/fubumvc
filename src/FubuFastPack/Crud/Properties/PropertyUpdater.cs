using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuFastPack.Domain;
using FubuFastPack.Persistence;
using FubuFastPack.Validation;
using FubuLocalization;
using FubuMVC.Core.UI.Security;
using FubuValidation;

namespace FubuFastPack.Crud.Properties
{
    public class PropertyUpdater<TEntity> : IPropertyUpdater<TEntity> where TEntity : DomainEntity
    {
        private readonly IFieldAccessService _fieldAccess;
        private readonly IList<IPropertyHandler<TEntity>> _handlers = new List<IPropertyHandler<TEntity>>();
        private readonly IPropertyUpdateLogger<TEntity> _logger;
        private readonly IRepository _repository;
        private readonly IValidator _validator;

        public PropertyUpdater(IFieldAccessService fieldAccess, IRepository repository, IValidator validator,
                               IPropertyUpdateLogger<TEntity> logger, ISimplePropertyHandler<TEntity> simpleHandler,
                               IEnumerable<IPropertyHandler<TEntity>> handlers)
        {
            _fieldAccess = fieldAccess;
            _repository = repository;
            _validator = validator;
            _logger = logger;
            _handlers.AddRange(handlers);

            _handlers.Add(simpleHandler);
        }

        public UpdatePropertyResultViewModel EditProperty(UpdatePropertyModel<TEntity> updatePropertyModel)
        {
            var model = _repository.Find<TEntity>(updatePropertyModel.Id);
            // TODO -- what if it isn't found?    
            // " ? "
            //updatePropertyModel.Formatter = _displayFormatter;

            EditPropertyResult editResult;
            try
            {
                var handler = _handlers.FirstOrDefault(x => x.CanEdit(updatePropertyModel));

                if (handler == null)
                {
                    throw new ApplicationException(
                        "No handler for property {0} on type {1}".ToFormat(updatePropertyModel.PropertyName,
                                                                           typeof(Entity).FullName));
                }

                var property = handler.FindProperty(updatePropertyModel);

                var rights = _fieldAccess.RightsFor(model, property.Property);
                if (!rights.Write)
                {
                    return new UpdatePropertyResultViewModel
                           {
                               errors =
                                   new[] { new ValidationError(property.Property.Name, FastPackKeys.NOT_AUTHORIZED.ToString()), },
                               message = FastPackKeys.NOT_AUTHORIZED.ToString(),
                               refresh = false,
                               success = false
                           };
                }


                editResult = handler.EditProperty(updatePropertyModel, model);
            }
            catch (FormatException ex)
            {
                return createResultForConversionFailure(updatePropertyModel, ex);
            }
            catch (InvalidPropertyConversionException ex)
            {
                return createResultForConversionFailure(updatePropertyModel, ex);
            }

            if (editResult.WasNotApplied)
            {
                return new UpdatePropertyResultViewModel
                       {
                           success = false,
                           errors = editResult.ToValidationErrors()
                       };
            }

            var returnValue = propertySaveResult(model, editResult);
            if (returnValue.success)
            {
                _logger.Log(model, editResult);
            }

            return returnValue;
        }

        private static UpdatePropertyResultViewModel createResultForConversionFailure(
            UpdatePropertyModel<TEntity> updatePropertyModel, Exception ex)
        {
            var message = ex.Message;
            var accessor = PropertyUtility.FindPropertyByName<TEntity>(updatePropertyModel.PropertyName);
            var propertyName = LocalizationManager.GetHeader(accessor.InnerProperty);

            var error = new ValidationError(propertyName, message);
            return new UpdatePropertyResultViewModel
                   {
                       success = false,
                       errors = new[] { error }
                   };
        }

        private UpdatePropertyResultViewModel propertySaveResult(TEntity entity, EditPropertyResult valueToDisplay)
        {
            var notification = _validator.Validate(entity);
            var response = new UpdatePropertyResultViewModel(notification, entity, valueToDisplay.NewValue);

            if (notification.IsValid())
            {
                _repository.Save(entity);
            }
            else
            {
                response.errors = notification.ToValidationErrors();
                response.success = false;
                _repository.RejectChanges(entity);
            }

            return response;
        }
    }
}