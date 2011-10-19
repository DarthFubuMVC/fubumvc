using System;
using System.Linq.Expressions;
using FubuCore.Reflection;

namespace FubuMVC.Core.UI.Configuration
{
    public class FormDef
    {
        public Type ModelType { get; set; }
        public int Id { get; set; }

        public bool IsInBound { get; set; }

        public bool Equals(FormDef other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(FormDef)) return false;
            return Equals((FormDef)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class AccessorDef
    {
        public Accessor Accessor { get; set; }
        public Type ModelType { get; set; }

        public static AccessorDef For<T>(Expression<Func<T, object>> expression)
        {
            return new AccessorDef
            {
                Accessor = expression.ToAccessor(),
                ModelType = typeof(T)
            };
        }

        public bool Equals(AccessorDef other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Accessor, Accessor) && Equals(other.ModelType, ModelType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(AccessorDef)) return false;
            return Equals((AccessorDef)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Accessor != null ? Accessor.GetHashCode() : 0) * 397) ^
                       (ModelType != null ? ModelType.GetHashCode() : 0);
            }
        }

        public bool Is<T>()
        {
            return Accessor.PropertyType.Equals(typeof(T));
        }
    }
}