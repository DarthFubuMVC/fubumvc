using System.Collections.Generic;
using FubuCore.Descriptions;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Ajax
{
    [Title("AjaxContinuationWriter")]
    public class AjaxContinuationWriter<T> : IMediaWriter<T> where T : AjaxContinuation
    {
        // TODO -- pull in IJsonWriter once.
        public void Write(string mimeType, IFubuRequestContext context, T resource)
        {
            context.Services.GetInstance<IJsonWriter>().Write(resource.ToDictionary(), mimeType);
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