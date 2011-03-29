using System;
using System.Reflection;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuFastPack.Domain;
using FubuFastPack.Persistence;

namespace FubuFastPack.Crud
{
    public class EntityConversionFamily : IConverterFamily
    {
        public bool Matches(PropertyInfo property)
        {
            return property.PropertyType.CanBeCastTo<DomainEntity>() 
                && !property.PropertyType.HasAttribute<IgnoreEntityInBindingAttribute>();
        }

        public ValueConverter Build(IValueConverterRegistry registry, PropertyInfo property)
        {
            var entityType = property.PropertyType;

            return request =>
            {
                var id = request.ValueAs<Guid?>();
                return id.HasValue ? request.Service<IRepository>().Find(entityType, id.Value) : null;
            };
        }
    }
}