using System.Collections.Generic;
using System.ComponentModel;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Core.Runtime
{
    [Description("Json serialization with the DataContractJsonSerializer")]
    public class DataContractJsonSerializer : IFormatter
    {
        public T Read<T>(IFubuRequestContext context)
        {
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof (T));
            return (T) serializer.ReadObject(context.Request.Input);
        }

        public void Write<T>(IFubuRequestContext context, T resource, string mimeType)
        {
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof (T));

            context.Writer.Write(mimeType, stream => { serializer.WriteObject(stream, resource); });
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