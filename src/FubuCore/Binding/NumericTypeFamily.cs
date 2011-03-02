using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace FubuCore.Binding
{
    public class NumericTypeFamily : IConverterFamily
    {
        private static CultureInfo _culture;
        
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
                               if (context.PropertyValue.ToString().IsValidNumber())
                               {
                                   var valueToConvert = removeNumericGroupSeparator(context.PropertyValue.ToString());
                                   return converter.ConvertFrom(valueToConvert);
                               }
                           }

                           return converter.ConvertFrom(context.PropertyValue);
                       };
        }

        private static string removeNumericGroupSeparator(string value)
        {
            _culture = Thread.CurrentThread.CurrentCulture;
            var numberSeparator = _culture.NumberFormat.NumberGroupSeparator;
            return value.Replace(numberSeparator, "");
        }
    }
}