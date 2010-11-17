using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Web;
using FubuCore.Binding;
using System.Linq;

namespace FubuCore.Binding
{
    public class ValueConverterRegistry : IValueConverterRegistry
    {
        private readonly List<IConverterFamily> _families = new List<IConverterFamily>();

        public ValueConverterRegistry(IConverterFamily[] families)
        {
            _families.AddRange(families);

            addPolicies();
        }

        public IEnumerable<IConverterFamily> Families { get { return _families; } }

        public ValueConverter FindConverter(PropertyInfo property)
        {
            IConverterFamily family = _families.FirstOrDefault(x => x.Matches(property));
            return family == null ? null : family.Build(this, property);
        }

        private void addPolicies()
        {
            Add<PassthroughConverter<HttpPostedFileBase>>();
            Add<PassthroughConverter<HttpFileCollectionWrapper>>();
            Add<PassthroughConverter<HttpCookie>>();
            Add<ASPNetObjectConversionFamily>();

            Add<ExpandEnvironmentVariablesFamily>();
            Add<MapFromWebPathFamily>();
            Add<MapWebToPhysicalPathFamily>();
            Add<ResolveConnectionStringFamily>();

            Add<BooleanFamily>();
            Add<BasicTypeConverter>();
        }

        public void Add<T>() where T : IConverterFamily, new()
        {
            _families.Add(new T());
        }

    }

    public class BasicTypeConverter : IConverterFamily
    {
        public bool Matches(PropertyInfo property)
        {
            try
            {
                return TypeDescriptor.GetConverter(property.PropertyType).CanConvertFrom(typeof (string));
            }
            catch (Exception)
            {
                return false;
            }
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
                }

                return converter.ConvertFrom(context.PropertyValue);
            };
        }
    }
}