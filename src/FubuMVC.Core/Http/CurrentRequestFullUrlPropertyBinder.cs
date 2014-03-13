using System.ComponentModel;
using System.Reflection;
using FubuCore.Binding;
using FubuCore.Descriptions;
using FubuCore.Reflection;

namespace FubuMVC.Core.Http
{
    [Title("FullUrl Property")]
    [Description("Binds the full url of the current request to an string property called FullUrl")]
    public class CurrentRequestFullUrlPropertyBinder : IPropertyBinder
    {
        private static readonly string PropertyName =
            ReflectionHelper.GetMethod<IHttpRequest>(r => r.FullUrl()).Name;

        public bool Matches(PropertyInfo property)
        {
            return property.Name.Equals(PropertyName) 
                    && property.PropertyType.FullName == typeof(string).FullName;
        }

        public void Bind(PropertyInfo property, IBindingContext context)
        {
            var request = context.Service<IHttpRequest>();
            property.SetValue(context.Object, request.FullUrl(), null);
        }
    }
}