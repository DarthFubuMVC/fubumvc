using System.Reflection;
using System.Web;
using FubuCore.Binding;
using FubuCore;

namespace FubuMVC.Core.Runtime
{
    public class AspNetPassthroughConverter : IConverterFamily
    {
        public bool Matches(PropertyInfo property)
        {
            return property.PropertyType.IsInNamespace(typeof (HttpPostedFileBase).Namespace);
        }

        public ValueConverter Build(IValueConverterRegistry registry, PropertyInfo property)
        {
            return typeof (PassthroughConverter<>).CloseAndBuildAs<ValueConverter>(property.PropertyType);
        }
    }
}