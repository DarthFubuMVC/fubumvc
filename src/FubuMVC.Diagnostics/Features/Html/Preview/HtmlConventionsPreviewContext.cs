using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace FubuMVC.Diagnostics.Features.Html.Preview
{
    public class HtmlConventionsPreviewContext
    {
        private readonly PropertyInfo[] _properties;

        public HtmlConventionsPreviewContext(string path, Type modelType, object instance, IEnumerable<PropertyInfo> propertyChain)
        {
            Path = path;
            ModelType = modelType;
            Instance = instance;
            PropertyChain = propertyChain;
            _properties = modelType.GetProperties();
        }

        public string Path { get; private set; }
        public Type ModelType { get; private set; }
        public object Instance { get; private set; }

        public IEnumerable<PropertyInfo> Properties
        {
            get { return _properties; }
        }

        public IEnumerable<PropertyInfo> PropertyChain { get; private set; }

        public IEnumerable<PropertyInfo> NonConvertibleProperties()
        {
            return _properties
                .Where(p => !TypeDescriptor.GetConverter(p.PropertyType).CanConvertFrom(typeof(string)));
        }

        public IEnumerable<PropertyInfo> SimpleProperties()
        {
            return _properties.Except(NonConvertibleProperties());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (HtmlConventionsPreviewContext)) return false;
            return Equals((HtmlConventionsPreviewContext) obj);
        }

        public bool Equals(HtmlConventionsPreviewContext other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.ModelType, ModelType);
        }

        public override int GetHashCode()
        {
            return ModelType.GetHashCode();
        }
    }
}