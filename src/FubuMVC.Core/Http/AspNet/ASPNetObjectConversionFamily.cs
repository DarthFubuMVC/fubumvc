using System.ComponentModel;
using System.Reflection;
using FubuCore.Binding;
using FubuCore.Descriptions;

namespace FubuMVC.Core.Http.AspNet
{
    [Title("ASP.Net Request Properties")]
    [Description("Passthrough conversion of any property value that matches a property on HttpRequestBase")]
    public class AspNetObjectConversionFamily : StatelessConverter
    {
        public override bool Matches(PropertyInfo property)
        {
            return RequestPropertyValueSource.IsSystemProperty(property);
        }

        public override object Convert(IPropertyContext context)
        {
            return context.RawValueFromRequest.RawValue;
        }
    }
}