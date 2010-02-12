using System.Reflection;

namespace FubuMVC.Core.Models
{
    public interface IPropertyBinder
    {
        bool Matches(PropertyInfo property);
        void Bind(PropertyInfo property, IBindingContext context);
    }
}