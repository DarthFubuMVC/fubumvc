using System.Collections.Generic;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Ajax
{
    public class AjaxContinuationWriter<T> : IMediaWriter<T> where T : AjaxContinuation
    {
        private readonly IJsonWriter _writer;

        public AjaxContinuationWriter(IJsonWriter writer)
        {
            _writer = writer;
        }

        public void Write(string mimeType, T resource)
        {
            _writer.Write(resource.ToDictionary(), mimeType);
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