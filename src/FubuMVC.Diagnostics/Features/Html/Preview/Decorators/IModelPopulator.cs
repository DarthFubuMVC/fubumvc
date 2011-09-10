using System.Collections.Generic;
using System.Reflection;

namespace FubuMVC.Diagnostics.Features.Html.Preview.Decorators
{
    public interface IModelPopulator
    {
        void PopulateInstance(object instance, IEnumerable<PropertyInfo> properties);
    }
}