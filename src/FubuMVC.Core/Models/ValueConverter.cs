using System.Reflection;

namespace FubuMVC.Core.Models
{
    public class RawValue
    {
        public PropertyInfo Property;
        public object Value;

        public IBindingContext Context;
    }

    public delegate object ValueConverter(RawValue value);
}