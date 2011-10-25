using System.Reflection;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Resources.Etags
{
    public class ResourcePathAttribute : BindingAttribute
    {
        public override void Bind(PropertyInfo property, IBindingContext context)
        {
            var resource = context.Service<ICurrentHttpRequest>().RelativeUrl().WithoutQueryString();
            property.SetValue(context.Object, resource, null);
        }
    }
}