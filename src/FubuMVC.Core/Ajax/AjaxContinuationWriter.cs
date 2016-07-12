using System.Collections.Generic;
using System.Threading.Tasks;
using FubuCore.Descriptions;
using FubuMVC.Core.Json;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Ajax
{
    [Title("AjaxContinuationWriter")]
    public class AjaxContinuationWriter<T> : IMediaWriter<T> where T : AjaxContinuation
    {
        public Task Write(string mimeType, IFubuRequestContext context, T resource)
        {
            var serializer = context.Services.GetInstance<IJsonSerializer>();
            var json = serializer.Serialize(resource.ToDictionary());
            return context.Writer.Write(mimeType, json);
        }

        public IEnumerable<string> Mimetypes
        {
            get
            {
                yield return "application/json";
                yield return "text/json";
            }
        }
    }
}