using System.ComponentModel;
using System.Reflection;
using System.Web;
using FubuCore.Binding;
using FubuCore;
using FubuCore.Descriptions;

namespace FubuMVC.Core.Runtime
{
    [Title("ASP.Net Passthrough")]
    [Description("Simply passes through any ASP.Net objects through without doing any conversion")]
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