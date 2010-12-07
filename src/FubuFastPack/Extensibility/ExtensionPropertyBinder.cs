using System;
using System.Reflection;
using FubuCore;
using FubuCore.Binding;

namespace FubuFastPack.Extensibility
{
    public class ExtensionPropertyBinder : IPropertyBinder
    {
        // This only applies to properties that close Extends<>
        public bool Matches(PropertyInfo property)
        {
            return property.PropertyType.Closes(typeof(Extends<>));
        }

        public void Bind(PropertyInfo property, IBindingContext context)
        {
            Type entityType = property.PropertyType.GetGenericArguments()[0];

            // If there is no Extends<> for the entity type, do nothing
            if (!ExtensionProperties.HasExtensionFor(entityType))
            {
                return;
            }

            Type extensionType = ExtensionProperties.ExtensionFor(entityType);

            // direct the FubuMVC model binding to resolve an object of the
            // extensionType using "entityType.Name" as the prefix on the form data,
            // and place the newly created object using the specified property
            context.BindChild(property, extensionType, entityType.Name);
        }
    }
}