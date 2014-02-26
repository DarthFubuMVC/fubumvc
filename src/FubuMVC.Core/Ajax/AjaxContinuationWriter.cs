using System.Collections.Generic;
using FubuCore.Descriptions;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Ajax
{
    [Title("AjaxContinuationWriter")]
    public class AjaxContinuationWriter<T> : IMediaWriter<T> where T : AjaxContinuation
    {
        private readonly JsonSerializer serializer = new JsonSerializer();

        public void Write(string mimeType, IFubuRequestContext context, T resource)
        {
            serializer.Write(context, resource.ToDictionary(), mimeType);
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