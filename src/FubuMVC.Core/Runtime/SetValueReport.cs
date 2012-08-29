using System;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Logging;

namespace FubuMVC.Core.Runtime
{
    public class SetValueReport : LogRecord, DescribesItself
    {
        public SetValueReport(object value)
        {
            Type = value.GetType();
            Value = value;
        }

        public SetValueReport()
        {
        }

        public Type Type { get; set; }
        public object Value { get; set; }

        public static SetValueReport For<T>(T value)
        {
            return new SetValueReport{
                Type = typeof (T),
                Value = value
            };
        }

        public bool Equals(SetValueReport other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Type, Type) && Equals(other.Value, Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (SetValueReport)) return false;
            return Equals((SetValueReport) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Type != null ? Type.GetHashCode() : 0)*397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }

        public void Describe(Description description)
        {
            description.Title = "Setting value of {0} in IFubuRequest".ToFormat(Type.Name);
            description.Properties["Value"] = Value.ToString();
        }

        public override string ToString()
        {
            return string.Format("Type: {0}, Value: {1}", Type, Value);
        }
    }
}