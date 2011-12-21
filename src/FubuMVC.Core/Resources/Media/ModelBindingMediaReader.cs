using System.Collections.Generic;
using FubuCore.Binding;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Media
{
    public class ModelBindingMediaReader<T> : IMediaReader<T>
    {
        private readonly IBindingContext _context;
        private readonly IObjectResolver _resolver;

        public ModelBindingMediaReader(IObjectResolver resolver, IBindingContext context)
        {
            _resolver = resolver;
            _context = context;
        }

        public T Read(string mimeType)
        {
            var result = _resolver.BindModel(typeof (T), _context);
            return (T) result.Value;
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