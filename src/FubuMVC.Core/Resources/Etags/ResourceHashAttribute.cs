using System.Reflection;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Resources.Etags
{
    public class ResourceHashAttribute : BindingAttribute
    {
        public override void Bind(PropertyInfo property, IBindingContext context)
        {
            var chain = context.Service<ICurrentChain>();
            var resource = ResourceHash.For(new VaryByResource(chain));
            
            property.SetValue(context.Object, resource, null);
        }
    }
}