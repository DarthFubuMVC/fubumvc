using System.Net;
using System.Reflection;
using FubuCore.Binding;
using FubuMVC.Core.Http;

namespace AspNetApplication.ServerSideEvents
{
    public class HeaderValueAttribute : BindingAttribute
    {
        private readonly string _headerName;

        public HeaderValueAttribute(string headerName)
        {
            _headerName = headerName;
        }

        public HeaderValueAttribute(HttpRequestHeader header)
        {
            _headerName = HttpRequestHeaders.HeaderNameFor(header);
        }

        public override void Bind(PropertyInfo property, IBindingContext context)
        {
            context.Service<IRequestHeaders>().Value<string>(_headerName, val =>
            {
                property.SetValue(context.Object, val, null);
            });
        }
    }
}