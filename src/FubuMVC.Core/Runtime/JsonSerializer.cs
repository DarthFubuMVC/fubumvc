using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Script.Serialization;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Core.Runtime
{
    [Description("Json serialization with the built in JavaScriptSerializer")]
    public class JsonSerializer : IFormatter
    {
        public T Read<T>(IFubuRequestContext context)
        {
            var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            return serializer.Deserialize<T>(context.Request.InputText());
        }

        public virtual void Write<T>(IFubuRequestContext context, T resource, string mimeType)
        {
            var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            var text = serializer.Serialize(resource);
            context.Writer.Write(mimeType, text);
        }

        public IEnumerable<string> MatchingMimetypes
        {
            get
            {
                yield return "application/json";
                yield return "text/json";
            }
        }
    }
}