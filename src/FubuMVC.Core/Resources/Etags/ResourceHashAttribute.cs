using System.Reflection;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Resources.Etags
{
    public class ResourceHashAttribute : BindingAttribute
    {
        public override void Bind(PropertyInfo property, IBindingContext context)
        {
            var resource = context.Service<ICurrentChain>().ResourceHash();
            property.SetValue(context.Object, resource, null);
        }
    }
}