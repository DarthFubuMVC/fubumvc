using System.Linq;
using System.Net;
using System.Reflection;
using FubuCore.Binding;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Runtime
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
            var value = context.Service<IHttpRequest>().GetHeader(_headerName).FirstOrDefault();
            property.SetValue(context.Object, value, null);
        }
    }
}