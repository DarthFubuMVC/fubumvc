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
            return new JavaScriptSerializer().Deserialize<T>(context.Request.InputText());
        }

        public virtual void Write<T>(IFubuRequestContext context, T resource, string mimeType)
        {
            var text = new JavaScriptSerializer().Serialize(resource);
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