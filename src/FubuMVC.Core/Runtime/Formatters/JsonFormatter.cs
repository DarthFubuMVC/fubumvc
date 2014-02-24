using System.Collections.Generic;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Resources.Conneg;

namespace FubuMVC.Core.Runtime.Formatters
{
    [MarkedForTermination("Unnecessary when we merge the IJsonSerializer model")]
    // See the integration tests for conneg
    [MimeType("application/json", "text/json")]
    [Title("Json Serialization")]
    public class JsonFormatter : IFormatter
    {
        public void Write<T>(IFubuRequestContext context, T target, string mimeType)
        {
            context.Services.GetInstance<IJsonSerializer>().Write(context, target, mimeType);
        }

        public T Read<T>(IFubuRequestContext context)
        {
            return context.Services.GetInstance<IJsonSerializer>().Read<T>(context);
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