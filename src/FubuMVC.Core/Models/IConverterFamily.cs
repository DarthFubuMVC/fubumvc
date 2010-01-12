using System.Reflection;

namespace FubuMVC.Core.Models
{
    public interface IConverterFamily
    {
        bool Matches(PropertyInfo property);
        ValueConverter Build(IValueConverterRegistry registry, PropertyInfo property);
    }
}