using System;
using System.Collections.Generic;
using System.Reflection;
using FubuCore;

namespace FubuMVC.Diagnostics.Features.Html.Preview.Decorators
{
    public class ModelPopulator : IModelPopulator
    {
        public void PopulateInstance(object instance, IEnumerable<PropertyInfo> properties)
        {
            // TODO -- go compositional here? 
            foreach (var property in properties)
            {
                var propertyValue = exampleValue(property.PropertyType);
                if (propertyValue != null)
                {
                    setProperty(property, instance, propertyValue);
                }
            }
        }

        private static void setProperty(PropertyInfo property, object instance, object propertyValue)
        {
            var setMethod = property.GetSetMethod();
            setMethod.Invoke(instance, new[] { propertyValue });
        }


        private static object exampleValue(Type propertyType)
        {
            if (propertyType == typeof(string)) return "Hello World";
            if (propertyType == typeof(char)) return 'F';
            if (propertyType == typeof(bool)) return true;
            if (propertyType.IsIntegerBased()) return 42;
            if (propertyType.IsFloatingPoint()) return 3.14;
            if (propertyType.IsDateTime()) return DateTime.Now;
            return null;
        }
    }
}