using System.Reflection;

namespace FubuMVC.Diagnostics.Features.Html.Preview
{
    public interface IPropertySourceGenerator
    {
        string SourceFor(PropertyInfo property);
    }
}