using System;
using System.Reflection;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuFastPack.Domain;

namespace FubuFastPack.Extensibility
{
    public class ExtensionPropertyBinder : IPropertyBinder
    {
        private static readonly string propertyName = ReflectionHelper.GetProperty<DomainEntity>(x => x.ExtendedProperties).Name;

        // This only applies to properties that close Extends<>
        public bool Matches(PropertyInfo property)
        {
            return property.Name == propertyName && property.DeclaringType == typeof(DomainEntity);
        }

        public void Bind(PropertyInfo property, IBindingContext context)
        {
            if (context.Object == null) return;

            Type entityType = context.Object.GetTrueType();
            
            // If there is no Extends<> for the entity type, do nothing
            if (!ExtensionProperties.HasExtensionFor(entityType))
            {
                return;
            }

            Type extensionType = ExtensionProperties.ExtensionFor(entityType);

            // direct the FubuMVC model binding to resolve an object of the
            // extensionType using "entityType.Name" as the prefix on the form data,
            // and place the newly created object using the specified property
            context.BindChild(property, extensionType, string.Empty);
        }
    }
}