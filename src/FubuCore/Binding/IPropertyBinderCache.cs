using System.Reflection;

namespace FubuMVC.Core.Models
{
    public interface IPropertyBinderCache
    {
        IPropertyBinder BinderFor(PropertyInfo property);
    }
}