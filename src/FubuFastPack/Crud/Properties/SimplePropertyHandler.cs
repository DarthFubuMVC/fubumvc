using System;
using FubuCore;
using FubuCore.Reflection;
using FubuFastPack.Domain;
using FubuFastPack.Validation;
using FubuLocalization;

namespace FubuFastPack.Crud.Properties
{
    public class SimplePropertyHandler<T> : ISimplePropertyHandler<T> where T : DomainEntity
    {
        private readonly IObjectConverter _converter;
        private readonly IDisplayFormatter _formatter;

        public SimplePropertyHandler(IObjectConverter converter, IDisplayFormatter formatter)
        {
            _converter = converter;
            _formatter = formatter;
        }

        public virtual EditPropertyResult EditProperty(UpdatePropertyModel<T> update, T entity)
        {
            var accessor = getAccessor(update);
            var oldValue = accessor.GetValue(entity);
            var newValueString = update.PropertyValue ?? string.Empty;
            object newValue = null;
            var targetType = accessor.PropertyType;
            try
            {
                newValue = _converter.FromString(newValueString, targetType);

                if (!PropertyUtility.IsChanged(targetType, newValue, oldValue))
                {
                    return EditPropertyResult.NotChangedResult();
                }
            }
            catch (Exception ex)
            {
                throw InvalidPropertyConversionException
                    .For(FastPackKeys.INVALID_TYPE_CONVERSION.ToFormat(newValueString,
                                                                       LocalizationManager.GetTextForType(
                                                                           targetType.Name.ToUpper())), ex);
            }

            accessor.SetValue(entity, newValue);
            var prevValue = oldValue == null ? string.Empty : oldValue.ToString();
            var result = new EditPropertyResult(accessor, typeof(T), prevValue, newValueString);

            if (!result.IsListAccessor())
            {
                //Used to be UpdatePropertyModel.formatter.GetDisplay.  We had an unused _flattener variable.  Why?
                result.PreviousValue = _formatter.GetDisplay(accessor, oldValue);
                result.NewValue = _formatter.GetDisplayForProperty(accessor, entity);
            }

            return result;
        }

        public virtual bool CanEdit(UpdatePropertyModel<T> update)
        {
            var accessor = getAccessor(update);
            return accessor != null;
        }

        public PropertyToUpdate FindProperty(UpdatePropertyModel<T> update)
        {
            return PropertyToUpdate.For<T>(getAccessor(update));
        }

        protected virtual Accessor getAccessor(UpdatePropertyModel<T> update)
        {
            return PropertyUtility.FindPropertyByName<T>(update.PropertyName);
        }
    }
}