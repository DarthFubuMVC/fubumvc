using System.Reflection;
using FubuCore.Binding;
using FubuCore.Reflection;

namespace FubuMVC.Core.Http
{
    public class CurrentRequestFullUrlPropertyBinder : IPropertyBinder
    {
        private static readonly string PropertyName =
            ReflectionHelper.GetMethod<ICurrentHttpRequest>(r => r.FullUrl()).Name;

        public bool Matches(PropertyInfo property)
        {
            return property.Name.Equals(PropertyName) 
                    && property.PropertyType.FullName == typeof(string).FullName;
        }

        public void Bind(PropertyInfo property, IBindingContext context)
        {
            var request = context.Service<ICurrentHttpRequest>();
            property.SetValue(context.Object, request.FullUrl(), null);
        }
    }
}