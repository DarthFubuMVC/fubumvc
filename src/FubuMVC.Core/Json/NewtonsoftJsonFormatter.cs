using System.Collections.Generic;
using System.ComponentModel;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Core.Json
{
    [Description("Json serialization with Newtonsoft.Json")]
    public class NewtonsoftJsonFormatter : IFormatter
    {
        public virtual void Write<T>(IFubuRequestContext context, T target, string mimeType)
        {
            var text = serializeData(context, target);
            context.Writer.Write(mimeType, text);
        }

        protected static string serializeData<T>(IFubuRequestContext context, T target)
        {
            var serializer = context.Services.GetInstance<IJsonSerializer>();
            var text = serializer.Serialize(target);
            return text;
        }

        public T Read<T>(IFubuRequestContext context)
        {
            var serializer = context.Services.GetInstance<IJsonSerializer>();
            return serializer.Deserialize<T>(context.Request.Input.ReadAllText());
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