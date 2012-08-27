using System;
using FubuCore.Descriptions;
using FubuCore.Logging;
using FubuCore;

namespace FubuMVC.Core.Runtime.Logging
{
    public class SetHeaderValue : LogRecord, DescribesItself
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public SetHeaderValue(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public bool Equals(SetHeaderValue other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Key, Key) && Equals(other.Value, Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (SetHeaderValue)) return false;
            return Equals((SetHeaderValue) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Key != null ? Key.GetHashCode() : 0)*397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }

        public void Describe(Description description)
        {
            description.Title = "Set header {0} = {1}".ToFormat(Key, Value);
        }

        public override string ToString()
        {
            return string.Format("Key: {0}, Value: {1}", Key, Value);
        }
    }
}