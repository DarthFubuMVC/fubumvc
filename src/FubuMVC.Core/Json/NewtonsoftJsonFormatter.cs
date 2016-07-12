using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Core.Json
{
    [Description("Json serialization with Newtonsoft.Json")]
    public class NewtonsoftJsonFormatter : IFormatter
    {
        public virtual Task Write<T>(IFubuRequestContext context, T target, string mimeType)
        {
            var text = serializeData(context, target);
            return context.Writer.Write(mimeType, text);
        }

        protected static string serializeData<T>(IFubuRequestContext context, T target)
        {
            var serializer = context.Services.GetInstance<IJsonSerializer>();
            var text = serializer.Serialize(target);
            return text;
        }

        public async Task<T> Read<T>(IFubuRequestContext context)
        {
            var serializer = context.Services.GetInstance<IJsonSerializer>();

            var reader = new StreamReader(context.Request.Input);
            var json = await reader.ReadToEndAsync().ConfigureAwait(false);

            return serializer.Deserialize<T>(json);
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