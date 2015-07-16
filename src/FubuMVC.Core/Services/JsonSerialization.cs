using System.IO;
using Newtonsoft.Json;

namespace FubuMVC.Core.Services
{
    public static class JsonSerialization
    {
        public static string ToJson(object o, bool indentedFormatting = false)
        {
            var serializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.All };

            if (indentedFormatting)
            {
                serializer.Formatting = Formatting.Indented;
            }

            var writer = new StringWriter();
            serializer.Serialize(writer, o);

            return writer.ToString();
        }
    }
}
