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
            // Address is a dependent property and we need to treat it as a nested object instead
            return property.PropertyType.CanBeCastTo<DomainEntity>() && !property.PropertyType.HasAttribute<IgnoreEntityInBindingAttribute>();
        }

        // TODO: [FubuBound("could use some fubu convenience love here")]
        public ValueConverter Build(IValueConverterRegistry registry, PropertyInfo property)
        {
            var entityType = property.PropertyType;

            // TODO -- come back to this.  Surely a better way.
            // May be some fubu changes
            return request =>
            {
                var rawString = request.PropertyValue.ToString();
                if (rawString.IsEmpty()) return null;

                var id = Guid.Parse(rawString);
                return (object)request.Service<IRepository>().Find(entityType, id);
            };
        }
    }
}