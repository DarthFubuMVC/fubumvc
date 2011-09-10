using System.Reflection;

namespace FubuMVC.Diagnostics.Features.Html.Preview
{
    public class PropertySourceGenerator : IPropertySourceGenerator
    {
        public string SourceFor(PropertyInfo property)
        {
            return string.Format("{0} {1} {2} {{ get; set; }}",
                                 property.GetGetMethod().IsPublic ? "public" : "internal",
                                 property.PropertyType.Name,
                                 property.Name);
        }
    }
}