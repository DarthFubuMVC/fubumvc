using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Json
{
    public class NewtonsoftJsonFormatter : IFormatter
    {
        public void Write<T>(IFubuRequestContext context, T target, string mimeType)
        {
            var serializer = context.Services.GetInstance<IJsonSerializer>();
            var text = serializer.Serialize(target);
            context.Writer.Write(mimeType, text);
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