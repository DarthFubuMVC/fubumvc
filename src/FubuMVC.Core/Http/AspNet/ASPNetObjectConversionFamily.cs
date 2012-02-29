using System.Reflection;
using FubuCore.Binding;

namespace FubuMVC.Core.Http.AspNet
{
    public class AspNetObjectConversionFamily : StatelessConverter
    {
        public override bool Matches(PropertyInfo property)
        {
            return AspNetAggregateDictionary.IsSystemProperty(property);
        }

        public override object Convert(IPropertyContext context)
        {
            return context.RawValueFromRequest;
        }
    }
}