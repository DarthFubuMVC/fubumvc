using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuFastPack.Domain;

namespace FubuFastPack.Crud
{
    public class ExtensionPropertyAccessor<TExtends> : Accessor where TExtends : new()
    {
        private readonly PropertyInfo _property;

        public ExtensionPropertyAccessor(PropertyInfo property)
        {
            _property = property;
        }

        public void SetValue(object target, object propertyValue)
        {
            var entity = (DomainEntity)target;
            if (entity.ExtendedProperties == null)
            {
                entity.ExtendedProperties = new TExtends();
            }

            _property.SetValue(entity.ExtendedProperties, propertyValue, null);
        }

        public object GetValue(object target)
        {
            var entity = (DomainEntity)target;
            if (entity.ExtendedProperties == null) return null;

            return _property.GetValue(entity.ExtendedProperties, null);
        }

        public Accessor GetChildAccessor<T>(Expression<Func<T, object>> expression)
        {
            throw new NotImplementedException();
        }

        public Expression<Func<T, object>> ToExpression<T>()
        {
            throw new NotImplementedException();
        }

        public Accessor Prepend(PropertyInfo property)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IValueGetter> Getters()
        {
            throw new NotImplementedException();
        }

        public string FieldName
        {
            get { throw new NotImplementedException(); }
        }

        public Type PropertyType
        {
            get { return _property.PropertyType; }
        }

        public PropertyInfo InnerProperty
        {
            get { return _property; }
        }

        public Type DeclaringType
        {
            get { return _property.PropertyType; }
        }

        public string Name
        {
            get { return _property.Name; }
        }

        public Type OwnerType
        {
            get { return typeof(TExtends); }
        }

        public string[] PropertyNames
        {
            get { throw new NotImplementedException(); }
        }
    }
}