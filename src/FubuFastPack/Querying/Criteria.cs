using System;
using System.Linq.Expressions;
using FubuCore.Reflection;

namespace FubuFastPack.Querying
{
    public class Criteria
    {
        public static Criteria For<T>(Expression<Func<T, object>> property, string op, string value)
        {
            return new Criteria()
                   {
                       op = op,
                       value = value,
                       property = ReflectionHelper.GetProperty(property).Name
                   };
        }


        public string property { get; set; }
        public string op { get; set; }
        public string value { get; set; }
        public bool valid
        {
            get
            {
                return (!string.IsNullOrEmpty(property) && !string.IsNullOrEmpty(op));
            }
        }

        public bool Equals(Criteria obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj.property, property) && Equals(obj.op, op) && Equals(obj.value, value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Criteria)) return false;
            return Equals((Criteria)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (property != null ? property.GetHashCode() : 0);
                result = (result * 397) ^ (op != null ? op.GetHashCode() : 0);
                result = (result * 397) ^ (value != null ? value.GetHashCode() : 0);
                return result;
            }
        }

        public override string ToString()
        {
            return string.Format("property: {0}, op: {1}, value: {2}", property, op, value);
        }
    }
}