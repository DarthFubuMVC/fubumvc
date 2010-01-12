using System.Reflection;

namespace FubuMVC.Core.Models
{
    public class RawValue
    {
        public PropertyInfo Property;
        public object Value;
    }

    public delegate object ValueConverter(RawValue value);
}