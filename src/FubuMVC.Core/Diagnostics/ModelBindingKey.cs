using System;
using FubuCore.Binding;

namespace FubuMVC.Core.Diagnostics
{
    public interface IModelBindingDetail
    {
    }

    public class ModelBindingException : IModelBindingDetail
    {
        public string StackTrace { get; set; }
    }

    public class ModelBindingKey : IModelBindingDetail
    {
        public string Key;
        public object Value;
        public RequestDataSource Source;
    }

    public class ModelBinderSelection : IModelBindingDetail
    {
        public Type ModelType { get; set; }
        public Type BinderType { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ModelBinderSelection)) return false;
            return Equals((ModelBinderSelection) obj);
        }

        public bool Equals(ModelBinderSelection other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.ModelType, ModelType) && Equals(other.BinderType, BinderType);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ModelType != null ? ModelType.GetHashCode() : 0)*397) ^ (BinderType != null ? BinderType.GetHashCode() : 0);
            }
        }
    }

    public class PropertyBinderSelection : IModelBindingDetail
    {
        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
        public Type BinderType { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (PropertyBinderSelection)) return false;
            return Equals((PropertyBinderSelection) obj);
        }

        public bool Equals(PropertyBinderSelection other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.PropertyName, PropertyName) && Equals(other.PropertyType, PropertyType) && Equals(other.BinderType, BinderType);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (PropertyName != null ? PropertyName.GetHashCode() : 0);
                result = (result*397) ^ (PropertyType != null ? PropertyType.GetHashCode() : 0);
                result = (result*397) ^ (BinderType != null ? BinderType.GetHashCode() : 0);
                return result;
            }
        }
    }

    public class ValueConverterSelection : IModelBindingDetail
    {
        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
        public Type ConverterType { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ValueConverterSelection)) return false;
            return Equals((ValueConverterSelection) obj);
        }

        public bool Equals(ValueConverterSelection other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.PropertyName, PropertyName) && Equals(other.PropertyType, PropertyType) && Equals(other.ConverterType, ConverterType);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (PropertyName != null ? PropertyName.GetHashCode() : 0);
                result = (result*397) ^ (PropertyType != null ? PropertyType.GetHashCode() : 0);
                result = (result*397) ^ (ConverterType != null ? ConverterType.GetHashCode() : 0);
                return result;
            }
        }
    }
}