using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

namespace FubuCore.Binding
{
    public class NumericTypeFamily : IConverterFamily
    {
        public bool Matches(PropertyInfo property)
        {
            return property.PropertyType.IsNumeric();
        }

        public ValueConverter Build(IValueConverterRegistry registry, PropertyInfo property)
        {
            var propertyType = property.PropertyType;

            return GetValueConverter(propertyType);
        }

        public static ValueConverter GetValueConverter(Type propertyType)
        {
            var converter = TypeDescriptor.GetConverter(propertyType);

          
            return context =>
                       {
                           if (context.PropertyValue != null)
                           {
                               if (context.PropertyValue.GetType() == propertyType)
                               {
                                   return context.PropertyValue;
                               }
                              
                               var valueToConvert = removeNumericGroupSeparator(context.PropertyValue.ToString());
                               return converter.ConvertFrom(valueToConvert);
                           }

                           return converter.ConvertFrom(context.PropertyValue);
                       };
        }

        private static string removeNumericGroupSeparator(string value)
        {
            var culture = Thread.CurrentThread.CurrentCulture;
            var numberSeparator = culture.NumberFormat.NumberGroupSeparator;
            return value.Replace(numberSeparator, "");
        }
    }
}