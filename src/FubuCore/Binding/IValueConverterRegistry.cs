using System.Reflection;

namespace FubuMVC.Core.Models
{
    public interface IValueConverterRegistry
    {
        ValueConverter FindConverter(PropertyInfo property);
    }
}