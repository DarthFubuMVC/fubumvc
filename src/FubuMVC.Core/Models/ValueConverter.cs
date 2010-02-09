using System.Reflection;

namespace FubuMVC.Core.Models
{
    public class RawValue
    {
        public PropertyInfo Property;
        public object Value;

        // TODO -- need to put this back
    }

    public delegate object ValueConverter(RawValue value);
}