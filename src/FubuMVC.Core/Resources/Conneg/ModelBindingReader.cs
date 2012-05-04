using System.Collections.Generic;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg
{
    public class ModelBindingReader<T> : IReader<T> where T : class
    {
        private readonly IFubuRequest _request;

        public ModelBindingReader(IFubuRequest request)
        {
            _request = request;
        }

        public T Read(string mimeType)
        {
            _request.Clear(typeof(T));
            return _request.Get<T>();
        }

        public IEnumerable<string> Mimetypes
        {
            get
            {
                yield return MimeType.HttpFormMimetype;
                yield return MimeType.MultipartMimetype;
            }
        }
    }
}